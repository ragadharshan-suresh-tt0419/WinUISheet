using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL
{
    internal class Row
    {
        private int rowIdx;
        private Cell head;
        private Row next;

        public Row(int r)
        {
            this.head = null;
            this.next = null;
            this.rowIdx = r;
        }

        public int getRowIdx()
        {
            return rowIdx;
        }
        
        public void setRowIdx(int r)
        {
            this.rowIdx = r;
        }

        public Cell getHead()
        {
            return head;
        }

        public void setHead(Cell h)
        {
            this.head = h;
        }

        public Row getNext()
        {
            return next;
        }

        public void setNext(Row n)
        {
            this.next = n;
        }
    }
}
