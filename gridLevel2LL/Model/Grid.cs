using gridLevel2LL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Model
{
    internal class Grid : IGrid
    {
        Row head;

        public int TotalRows { get; private set; }
        public int TotalColumns { get; private set; }

        public Grid(int initialRows, int initialColumns)
        {
            TotalRows = initialRows;
            TotalColumns = initialColumns;
            head = null;
        }

        public void Clear()
        {
            head = null;
        }

        public void SetDimensions(int rows, int cols)
        {
            TotalRows = rows;
            TotalColumns = cols;
        }

        public Row GetOrCreateRow(int index)
        {
            Row curr = head;
            Row prev = null;

            while (curr != null && curr.getRowIdx() < index)
            {
                prev = curr;
                curr = curr.getNext();
            }
            if (curr != null && curr.getRowIdx() == index)
            {
                return curr;
            }

            Row newRow = new Row(index);

            if (prev == null)
            {
                newRow.setNext(head);
                head = newRow;
            }
            else
            {
                newRow.setNext(prev.getNext());
                prev.setNext(newRow);
            }

            return newRow;
        }

        public void InsertCell(int r, int c, string value)
        {
            if (r < 0 || c < 0)
            {
                throw new ArgumentOutOfRangeException("Row and column indices must be non-negative");
            }

            if (value == null)
            {
                value = string.Empty;
            }

            Row row = GetOrCreateRow(r);

            Cell curr = row.getHead();
            Cell prev = null;

            while (curr != null && curr.GetColIdx() < c)
            {
                prev = curr;
                curr = curr.GetNext();
            }

            if (curr != null && curr.GetColIdx() == c)
            {
                curr.SetValue(value.ToString());
                return;
            }

            Cell newCell = new Cell(r, c, value.ToString());

            if (prev == null)
            {
                newCell.SetNext(row.getHead());
                row.setHead(newCell);
            }
            else
            {
                newCell.SetNext(prev.GetNext());
                prev.SetNext(newCell);
            }
        }

        public void updateIndicesRow(Row startRow, int delta)
        {
            Row temp = startRow;

            while (temp != null)
            {
                temp.setRowIdx(temp.getRowIdx() + delta);

                Cell cell = temp.getHead();
                while (cell != null)
                {
                    cell.SetRowIdx(cell.GetRowIdx() + delta);
                    cell = cell.GetNext();
                }

                temp = temp.getNext();
            }
        }

        public void InsertRow(int index)
        {
            if (index < 0 || index > TotalRows)
            {
                throw new ArgumentOutOfRangeException($"Index must be between 0 and {TotalRows}");
            }

            Row curr = head;
            Row prev = null;

            while (curr != null && curr.getRowIdx() < index)
            {
                prev = curr;
                curr = curr.getNext();
            }

            updateIndicesRow(curr, +1);

            Row newRow = new Row(index);
            if (prev == null)
            {
                newRow.setNext(head);
                head = newRow;
            }
            else
            {
                newRow.setNext(prev.getNext());
                prev.setNext(newRow);
            }

            TotalRows++;
        }

        public void DeleteRow(int index)
        {
            if (index < 0 || index >= TotalRows)
            {
                throw new ArgumentOutOfRangeException($"Index must be between 0 and {TotalRows - 1}");
            }

            if (TotalRows == 0)
            {
                return;
            }

            Row curr = head;
            Row prev = null;

            while (curr != null && curr.getRowIdx() < index)
            {
                prev = curr;
                curr = curr.getNext();
            }

            if (curr == null || curr.getRowIdx() != index)
            {
                updateIndicesRow(curr, -1);
                TotalRows--;
                return;
            }

            if (prev == null)
            {
                head = curr.getNext();
            }
            else
            {
                prev.setNext(curr.getNext());
            }

            updateIndicesRow(curr.getNext(), -1);

            TotalRows--;
        }

        public void updateIndicesCol(int index, int delta)
        {
            Row row = head;

            while (row != null)
            {
                Cell cell = row.getHead();

                while (cell != null)
                {
                    if (cell.GetColIdx() >= index)
                    {
                        cell.SetColIdx(cell.GetColIdx() + delta);
                    }
                    cell = cell.GetNext();
                }
                row = row.getNext();
            }
        }

        public void InsertColumn(int index)
        {
            if (index < 0 || index > TotalColumns)
            {
                throw new ArgumentOutOfRangeException($"Index must be between 0 and {TotalColumns}");
            }

            updateIndicesCol(index, +1);

            TotalColumns++;
        }

        public void DeleteColumn(int index)
        {
            if (index < 0 || index >= TotalColumns)
            {
                throw new ArgumentOutOfRangeException($"Index must be between 0 and {TotalColumns - 1}");
            }

            if (TotalColumns == 0)
            {
                return;
            }

            Row row = head;
            while (row != null)
            {
                Cell curr = row.getHead();
                Cell prev = null;

                while (curr != null)
                {
                    if (curr.GetColIdx() == index)
                    {
                        if (prev == null)
                        {
                            row.setHead(curr.GetNext());
                            curr = row.getHead();
                        }
                        else
                        {
                            prev.SetNext(curr.GetNext());
                            curr = prev.GetNext();
                        }
                    }
                    else
                    {
                        prev = curr;
                        curr = curr.GetNext();
                    }
                }

                row = row.getNext();
            }

            updateIndicesCol(index, -1);

            TotalColumns--;
        }

        public string GetCellValue(int r, int c)
        {
            Row row = head;
            while (row != null && row.getRowIdx() < r)
            {
                row = row.getNext();
            }

            if (row == null || row.getRowIdx() != r)
            {
                return "";
            }

            Cell cell = row.getHead();
            while (cell != null && cell.GetColIdx() < c)
            {
                cell = cell.GetNext();
            }

            if (cell != null && cell.GetColIdx() == c)
            {
                return cell.GetValue();
            }
            return "";
        }

        public List<(int row, int col, string value)> GetAllCells()
        {
            var result = new List<(int row, int col, string value)>();
            Row row = head;
            while (row != null)
            {
                Cell cell = row.getHead();
                while (cell != null)
                {
                    if (!string.IsNullOrEmpty(cell.GetValue()))
                        result.Add((row.getRowIdx(), cell.GetColIdx(), cell.GetValue()));
                    cell = cell.GetNext();
                }
                row = row.getNext();
            }
            return result;
        }
    }
}