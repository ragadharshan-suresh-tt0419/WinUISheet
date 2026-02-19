using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Data.Entities
{
    public class CellEntity
    {
        public int CellId { get; set; }
        public int RowId { get; set; }
        public int ColumnIndex { get; set; }
        public string Value { get; set; } = string.Empty;
        public RowEntity Row { get; set; }
    }
}
