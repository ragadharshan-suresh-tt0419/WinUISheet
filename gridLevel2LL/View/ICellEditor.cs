using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.View
{
    internal interface ICellEditor
    {
        void StartEditing(int row, int col);
        void CommitEdit();
        void CancelEdit();
        bool IsEditing();
        
    }
}
