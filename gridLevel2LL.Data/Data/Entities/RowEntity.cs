using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Data.Entities
{
    public class RowEntity
    {
        public int RowId { get; set; }  
        public int GridId { get; set; }
        public int RowIndex { get; set; }

        public GridEntity Grid { get; set; }

        public List<CellEntity> Cells { get; set; } = new List<CellEntity>();
    }
}
