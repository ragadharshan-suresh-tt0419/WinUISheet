using System;
using System.Collections.Generic;
using gridLevel2LL.Commands;
using gridLevel2LL.Model;
using gridLevel2LL.Services;
using System.Threading.Tasks;

namespace gridLevel2LL.ViewModel
{
    internal class GridChangedEventArgs : EventArgs
    {
        public enum ChangeType { RowInserted, RowDeleted, ColumnInserted, ColumnDeleted, CellUpdated, UndoRedo }
        public ChangeType type { get; }
        public GridChangedEventArgs(ChangeType type) { this.type = type; }
    }

    internal class GridViewModel
    {
        private Grid grid; 
        private CommandManager commandManager;
        private GridApiService apiService;

        public EventHandler<GridChangedEventArgs> GridChanged;
        public int currentGridId;

        public int TotalRows => grid.TotalRows;
        public int TotalColumns => grid.TotalColumns;
        public bool CanUndo => commandManager.canUndo;
        public bool CanRedo => commandManager.canRedo;

        public GridViewModel(IGrid grid)
        {
            this.grid = (Grid)grid;
            this.commandManager = new CommandManager();
            this.apiService = new GridApiService();
        }

        public async Task LoadFromApiAsync(int gridId)
        {
            var gridData = await apiService.GetGridDataAsync(gridId);
            currentGridId = gridId;

            grid.Clear();

            int minRows = 20;
            int minCols = 20;

            int maxRow = Math.Max(minRows, gridData.totalRows);
            int maxCol = Math.Max(minCols, gridData.totalColumns);

            if (gridData.cells != null)
            {
                foreach (var cell in gridData.cells)
                {
                    grid.InsertCell(cell.row, cell.col, cell.value);
                    if (cell.row + 1 > maxRow) maxRow = cell.row + 1;
                    if (cell.col + 1 > maxCol) maxCol = cell.col + 1;
                }
            }

            grid.SetDimensions(maxRow, maxCol);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.UndoRedo));
        }

        public async Task<int> CreateNewGridAsync(string name)
        {
            currentGridId = await apiService.CreateGridAsync(name);
            return currentGridId;
        }

        public string GetCellValue(int row, int col)
        {
            return grid.GetCellValue(row, col);
        }

        public async Task EditCell(int row, int col, string newValue)
        {
            string oldValue = grid.GetCellValue(row, col);
            if (newValue == oldValue) return;

            var command = new EditCellCommand(grid, row, col, oldValue, newValue);
            commandManager.Execute(command);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.CellUpdated));

            await apiService.UpdateCellAsync(currentGridId, row, col, newValue);
        }

        public async Task InsertRow(int index)
        {
            var command = new InsertRowCommand(grid, index);
            commandManager.Execute(command);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.RowInserted));
            await apiService.InsertRowAsync(currentGridId, index);
        }

        public async Task DeleteRow(int index)
        {
            var command = new DeleteRowCommand(grid, index);
            commandManager.Execute(command);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.RowDeleted));
            await apiService.DeleteRowAsync(currentGridId, index);
        }

        public async Task InsertColumn(int index)
        {
            var command = new InsertColumnCommand(grid, index);
            commandManager.Execute(command);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.ColumnInserted));
            await apiService.InsertColAsync(currentGridId, index);
        }

        public async Task DeleteColumn(int index)
        {
            var command = new DeleteColumnCommand(grid, index);
            commandManager.Execute(command);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.ColumnDeleted));
            await apiService.DeleteColAsync(currentGridId, index);
        }

        public async Task Undo()
        {
            if (!CanUndo) return;
            commandManager.Undo();

            await SyncToServerAsync();

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.UndoRedo));
        }

        public async Task Redo()
        {
            if (!CanRedo) return;
            commandManager.Redo();

            await SyncToServerAsync();

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.UndoRedo));
        }

        private async Task SyncToServerAsync()
        {
            var allCells = grid.GetAllCells();
            await apiService.SaveAllCellsAsync(currentGridId, grid.TotalRows, grid.TotalColumns, allCells);
        }
    }
}