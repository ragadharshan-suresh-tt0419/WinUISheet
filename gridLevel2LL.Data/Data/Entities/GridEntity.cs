using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Data.Entities
{
    public class GridEntity
    {
        public int GridId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }


        public List<RowEntity> Rows { get; set; } = new List<RowEntity>();
    }
}
