using gridLevel2LL.Model
;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Commands
{
    internal class DeleteColumnCommand : IGridCommand
    {
        private IGrid grid;
        private int index;
        private List<(int row, string value)> deletedCells;
        public DeleteColumnCommand(IGrid grid, int index)
        {
            this.grid = grid;
            this.index = index;
            this.deletedCells = new List<(int row, string value)>();
        }
        public void Execute()
        {
            deletedCells.Clear();
            for (int r = 0; r < grid.TotalRows; r++)
            {
                string value = grid.GetCellValue(r, this.index);
                if (!string.IsNullOrEmpty(value))
                {
                    deletedCells.Add((r, value));
                }
            }
            grid.DeleteColumn(this.index);
        }
        public void Undo()
        {
            grid.InsertColumn(this.index);
            foreach (var (row, value) in deletedCells)
            {
                grid.InsertCell(row, this.index, value);
            }
        }
    }
}
