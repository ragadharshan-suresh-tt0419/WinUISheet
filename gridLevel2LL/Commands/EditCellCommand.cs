using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gridLevel2LL.Model;

namespace gridLevel2LL.Commands
{
    internal class EditCellCommand : IGridCommand
    {

        private IGrid grid;
        private int row;
        private int col;
        private string newValue;
        private string oldValue;

        public EditCellCommand(IGrid grid, int row, int col, string oldValue, string newValue)
        {
            this.grid = grid;
            this.row = row;
            this.col = col;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public void Execute()
        {
            grid.InsertCell(this.row, this.col, this.newValue);
        }
        public void Undo()
        {
            grid.InsertCell(this.row, this.col, this.oldValue);
        }


    }
}
