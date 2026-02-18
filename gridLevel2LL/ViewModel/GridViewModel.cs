using gridLevel2LL.Commands;
using gridLevel2LL.Model__Backend_;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;

namespace gridLevel2LL.ViewModel
{

    internal class GridChangedEventArgs : EventArgs
    {
        public enum ChangeType
        {
            RowInserted,
            RowDeleted,
            ColumnInserted,
            ColumnDeleted,
            CellUpdated,
            UndoRedo
        }

        public ChangeType type { get; }

        public GridChangedEventArgs(ChangeType type)
        {
            this.type = type;
        }
    }
    internal class GridViewModel
    {
        private IGrid grid;
        private CommandManager commandManager;

        public EventHandler<GridChangedEventArgs> GridChanged;



        public int TotalRows => grid.TotalRows;
        public int TotalColumns => grid.TotalColumns;

        public bool CanUndo => commandManager.canUndo;
        public bool CanRedo => commandManager.canRedo;

        public GridViewModel(IGrid grid)
        {
            this.grid = grid;
            this.commandManager = new CommandManager();
        }

        public string GetCellValue(int row, int col)
        {
            return grid.GetCellValue(row, col);
        }

        public void EditCell(int row, int col, string newValue)
        {
            string oldValue = grid.GetCellValue(row, col);

            if (newValue == oldValue)
            {
                return;
            }

            var command = new EditCellCommand(grid, row, col, oldValue, newValue);
            commandManager.Execute(command);

        }


        public void InsertRow(int index)
        {
            var command = new InsertRowCommand(grid, index);
            commandManager.Execute(command);
            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.RowInserted));
        }


        public void DeleteRow(int index)
        {
            var command = new DeleteRowCommand(grid, index);
            commandManager.Execute(command);

            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.RowDeleted));
        }


        public void InsertColumn(int index)
        {
            var command = new InsertColumnCommand(grid, index);
            commandManager.Execute(command);
            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.ColumnInserted));
        }


        public void DeleteColumn(int index)
        {
            var command = new DeleteColumnCommand(grid, index);
            commandManager.Execute(command);
            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.ColumnDeleted));
        }


        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            commandManager.Undo();
            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.UndoRedo));
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                return;
            }
            commandManager.Redo();
            GridChanged?.Invoke(this, new GridChangedEventArgs(GridChangedEventArgs.ChangeType.UndoRedo));
        }
    }
}
