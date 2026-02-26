using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gridLevel2LL.Model;


namespace gridLevel2LL.Commands
{
    internal class InsertRowCommand : IGridCommand
    {
        private IGrid grid;
        private int index;
        public InsertRowCommand(IGrid grid, int index)
        {
            this.grid = grid;
            this.index= index;
        }
        public void Execute()
        {
            grid.InsertRow(this.index);
        }
        public void Undo()
        {
            grid.DeleteRow(this.index);
        }
    }
}
