using gridLevel2LL.Model__Backend_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Commands
{
    internal class InsertColumnCommand : IGridCommand
    {
        private IGrid grid;
        private int index;
        public InsertColumnCommand(IGrid grid, int index) {
            this.grid = grid;
            this.index = index;
        }
        public void Execute()
        {
            grid.InsertColumn(this.index);
        }
        public void Undo()
        {
            grid.DeleteColumn(this.index);
        }
    }
}
