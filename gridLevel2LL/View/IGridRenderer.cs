using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace gridLevel2LL.View
{
    internal interface IGridRenderer
    {
        public int CellWidth { get; }
        public int CellHeight { get; }
        void RenderAll();
        void RenderCell(int row, int col, string value);
        (int row, int col) GetCellFromPoint(Point point);
        Point GetCellPosition(int row, int col);
        void DrawGridLines();
        bool IsValidCell(int row, int col);
    }
}
