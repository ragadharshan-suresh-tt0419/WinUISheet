using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Commands
{
    internal interface IGridCommand
    {
        void Execute();
        void Undo();
    }
}
