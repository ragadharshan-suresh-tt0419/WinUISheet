using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Model
{
    internal interface IGrid
    {
        int TotalRows { get; }
        int TotalColumns { get; }
        void InsertCell(int r, int c, string value);
        string GetCellValue(int r, int c);
        void InsertRow(int index);
        void InsertColumn(int index);
        void DeleteRow(int index);
        void DeleteColumn(int index);
        void Clear();
        void SetDimensions(int rows, int cols);
    }
}