using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Model
{
    internal class Cell
    {
        private int rowIdx;
        private int colIdx;
        private string value;

        Cell next;

        public Cell(int rowId, int colId, string value)
        {
            this.rowIdx = rowId;
            this.colIdx = colId;
            this.value = value;
            this.next = null;
        }

        public int GetRowIdx()
        {
            return rowIdx;
        }

        public void SetRowIdx(int rowId)
        {
            this.rowIdx = rowId;
        }

        public int GetColIdx()
        {
            return colIdx;
        }

        public void SetColIdx(int colId)
        {
            this.colIdx = colId;
        }

        public string GetValue()
        {
            return value;
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        public Cell GetNext()
        {
            return next;
        }

        public void SetNext(Cell nextCell)
        {
            this.next = nextCell;
        }

    }
}
