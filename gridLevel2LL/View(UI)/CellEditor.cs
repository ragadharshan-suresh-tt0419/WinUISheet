using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

using Windows.Foundation;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;
using gridLevel2LL.View_UI_;
using gridLevel2LL.ViewModel;

namespace gridLevel2LL
{
    internal class CellEditor : ICellEditor
    {
        private Canvas canvas;
        private IGridRenderer renderer;
        private GridViewModel viewModel;
        private TextBox editingTextBox;

        private int currentEditingRow = -1;
        private int currentEditingColumn = -1;




        public CellEditor(IGridRenderer renderer, GridViewModel viewModel, Canvas canvas)
        {
            this.canvas = canvas;
            this.renderer = renderer;
            this.viewModel = viewModel;

            InitializeTextBox();
            PerformEvents();
           
        }


        private void InitializeTextBox()
        {
            editingTextBox = new TextBox()
            {
                Height = renderer.CellHeight,
                Width = renderer.CellWidth,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Collapsed,
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.White)
            };

            canvas.Children.Add(editingTextBox);
        }



        public void PerformEvents()
        {
            canvas.Tapped += Canvas_Tapped;
            editingTextBox.LostFocus += EditingTextBox_LostFocus;
            editingTextBox.KeyDown += EditingTextBox_KeyDown;

        }



        public void Canvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point selectedPoint = e.GetPosition(canvas);
            var (row, col) = renderer.GetCellFromPoint(selectedPoint);

            if(renderer.IsValidCell(row, col))
            {
                StartEditing(row, col);
                e.Handled = true;
            }
        }



        public void StartEditing(int row, int col)
        {
            SaveCurrentValue();

            currentEditingRow = row;
            currentEditingColumn = col;

            string currentValue = viewModel.GetCellValue(row, col);

            Point position = renderer.GetCellPosition(row, col);
            Canvas.SetLeft(editingTextBox, position.X);
            Canvas.SetTop(editingTextBox, position.Y);

            editingTextBox.Text = currentValue;
            editingTextBox.Visibility = Visibility.Visible;

            editingTextBox.Focus(FocusState.Programmatic);
            editingTextBox.SelectAll();

        }



        private void EditingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currentEditingRow != -1)
                CommitEdit();
        }


        public void EditingTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                CommitEdit();

                if(currentEditingRow < viewModel.TotalRows - 1)
                {
                    StartEditing(currentEditingRow + 1, currentEditingColumn);
                }
            }
            else if(e.Key == Windows.System.VirtualKey.Tab)
            {
                e.Handled = true;
                CommitEdit();

                if(currentEditingColumn < viewModel.TotalColumns - 1)
                {
                    StartEditing(currentEditingRow, currentEditingColumn + 1);
                }

                if(currentEditingRow < viewModel.TotalRows - 1)
                {
                    StartEditing(currentEditingRow + 1, 0);
                }
            }
            else if(e.Key == Windows.System.VirtualKey.Escape)
            {
                e.Handled  = true;
                CancelEdit();

            }
        }

        private void SaveCurrentValue()
        {
            if (currentEditingRow == -1 || currentEditingColumn == -1)
            {
                return;
            }

            string newValue = editingTextBox.Text;
            string oldValue = viewModel.GetCellValue(currentEditingRow, currentEditingColumn);

            if(newValue != oldValue)
            {
                viewModel.EditCell(currentEditingRow, currentEditingColumn, newValue);
            }
            renderer.RenderCell(currentEditingRow, currentEditingColumn, newValue);

            currentEditingRow = -1;
            currentEditingColumn = -1;
        }

        public void CommitEdit()
        {
            SaveCurrentValue();
            editingTextBox.Visibility = Visibility.Collapsed;

        }




        public void CancelEdit()
        {
            editingTextBox.Visibility = Visibility.Collapsed;
            currentEditingColumn = -1;
            currentEditingRow = -1;

            canvas.Focus(FocusState.Programmatic);
        }


        public bool IsEditing()
        {
            if(currentEditingRow != -1 && currentEditingColumn != -1)
            {
                return true;
            }
            return false;
        }
    }
}
