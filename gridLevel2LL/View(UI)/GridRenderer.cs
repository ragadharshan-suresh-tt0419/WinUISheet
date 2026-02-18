using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gridLevel2LL.View_UI_;
using gridLevel2LL.ViewModel;

namespace gridLevel2LL
{
    internal class GridRenderer : IGridRenderer
    {
        public Canvas canvas { get; private set; }
        public int CellHeight { get; private set; } 
        public int CellWidth { get; private set; }
        private GridViewModel viewModel;

        private List<Microsoft.UI.Xaml.Shapes.Line> GridLines;
        private Dictionary<(int rows, int cols), TextBlock> CellTextBlocks;
        


        public GridRenderer(Microsoft.UI.Xaml.Controls.Grid rootGrid, int cellHeight, int cellWidth, GridViewModel viewModel)
        {
            this.CellHeight = cellHeight;
            this.CellWidth = cellWidth;
            this.viewModel = viewModel;

            GridLines = new List<Microsoft.UI.Xaml.Shapes.Line>();
            CellTextBlocks = new Dictionary<(int rows, int cols), TextBlock>();

            InitializeCanvas(rootGrid);

            viewModel.GridChanged += OnGridChanged;
        }

        private void OnGridChanged(object sender, EventArgs e)
        {
            RenderAll();
        }

        private void InitializeCanvas(Microsoft.UI.Xaml.Controls.Grid rootGrid)
        {
            canvas = new Canvas
            {
                Height = CellHeight * viewModel.TotalRows,
                Width = CellWidth * viewModel.TotalColumns,
                Background = new SolidColorBrush(Microsoft.UI.Colors.White)
            };

            ScrollViewer scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = canvas
            };

            Microsoft.UI.Xaml.Controls.Grid.SetRow(scrollViewer, 1);
            rootGrid.Children.Add(scrollViewer);

        }




        public void DrawGridLines()
        {
            var nonLineChildren = new List<UIElement>();
            foreach (var child in canvas.Children)
            {
                if (!(child is Microsoft.UI.Xaml.Shapes.Line))
                {
                    nonLineChildren.Add(child);
                }
            }

            canvas.Children.Clear();
            GridLines.Clear();

            foreach (var child in nonLineChildren)
            {
                canvas.Children.Add(child);
            }

            canvas.UpdateLayout();

            int displayRows = Math.Max(1, viewModel.TotalRows);
            int displayCols = Math.Max(1, viewModel.TotalColumns);

            canvas.Height = displayRows * CellHeight;
            canvas.Width = displayCols * CellWidth;

            // horizontal lines
            for (int i = 0; i <= displayRows; i++)
            {
                Microsoft.UI.Xaml.Shapes.Line line = new Microsoft.UI.Xaml.Shapes.Line
                {
                    X1 = 0,
                    X2 = displayCols * CellWidth,
                    Y1 = i * CellHeight,
                    Y2 = i * CellHeight,
                    Stroke = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                    StrokeThickness = 1
                };
                GridLines.Add(line);
                canvas.Children.Add(line);

            }

            // Vertical lines
            for(int i = 0; i <= displayCols; i++)
            {
                Microsoft.UI.Xaml.Shapes.Line line = new Microsoft.UI.Xaml.Shapes.Line
                {
                    Y1 = 0,
                    Y2 = displayRows * CellHeight,
                    X1 = i * CellWidth,
                    X2 = i * CellWidth,
                    Stroke = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                    StrokeThickness = 1
                };
                GridLines.Add(line);
                canvas.Children.Add(line);
            }

            canvas.UpdateLayout();


        }



        public void RenderCell(int row, int col, string value)
        {
            if(CellTextBlocks.ContainsKey((row, col)))
            {
                canvas.Children.Remove(CellTextBlocks[(row, col)]);
                CellTextBlocks.Remove((row, col));
            }

            if (!string.IsNullOrEmpty(value))
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = value,
                    Height = CellHeight,
                    Width = CellWidth,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(5),
                    Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black)

                };
                Canvas.SetLeft(textBlock, col * CellWidth);
                Canvas.SetTop(textBlock, row * CellHeight);

                canvas.Children.Add(textBlock);
                CellTextBlocks[(row, col)] = textBlock;
            }
        }



        public void RenderAll()
        {
            foreach(var textBlock in CellTextBlocks.Values)
            {
                canvas.Children.Remove(textBlock);
            }
            CellTextBlocks.Clear();


            DrawGridLines();

            for(int r = 0; r < viewModel.TotalRows; r++)
            {
                for(int c = 0; c < viewModel.TotalColumns; c++)
                {
                    string value = viewModel.GetCellValue(r, c);
                    if (!string.IsNullOrEmpty(value))
                    {
                        RenderCell(r, c, value);
                    }
                }
            }

        }


        public (int row, int col) GetCellFromPoint(Point point)
        {
            int col = (int)point.X / CellWidth;
            int row = (int)point.Y / CellHeight;

            return (row, col);

        }


        public bool IsValidCell(int row, int col)
        {
            if(row >= 0 && row < viewModel.TotalRows && col >= 0 && col < viewModel.TotalColumns)
            {
                return true;
            }
            return false;
        }


        public Point GetCellPosition(int row, int col)
        {
            return new Point(col * CellWidth, row * CellHeight);
        }

    }
}
