using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gridLevel2LL.Model__Backend_;

namespace gridLevel2LL.Commands
{
    internal class DeleteRowCommand : IGridCommand
    {
        private IGrid grid;
        private int index;
        private List<(int col, string value)> deletedCells;

        public DeleteRowCommand(IGrid grid, int index)
        {
            this.grid = grid;
            this.index = index;
            this.deletedCells = new List<(int col, string value)>();
        }

        public void Execute()
        {
            deletedCells.Clear();

            for(int c = 0; c < grid.TotalColumns; c++)
            {
                string value = grid.GetCellValue(this.index, c);
                if(!string.IsNullOrEmpty(value))
                {
                    deletedCells.Add((c, value));
                }
            }
            grid.DeleteRow(this.index);
        }
        public void Undo()
        {
            grid.InsertRow(this.index);

            foreach(var (col, value) in deletedCells)
            {
                grid.InsertCell(this.index, col, value);
            }
        }
    }
}
