using gridLevel2LL.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gridLevel2LL.View_UI_;
using gridLevel2LL.Model__Backend_;

namespace gridLevel2LL
{
    internal class ControlPanel
    {
        Microsoft.UI.Xaml.Controls.Grid rootGrid;
        private GridViewModel viewModel;
        private ICellEditor editor;
        private TextBox indexBox;

        private Window parentWindow;
        

        public ControlPanel(Microsoft.UI.Xaml.Controls.Grid rootGrid, GridViewModel viewModel, ICellEditor editor)
        {
            this.rootGrid = rootGrid;
            this.viewModel = viewModel;
            this.editor = editor;
            this.indexBox = new TextBox
            {
                Width = 80,
                PlaceholderText = "Index"
            };

            CreatePanel();
        }

        private void CreatePanel()
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10),
                Spacing = 10,
                Background = new SolidColorBrush(Microsoft.UI.Colors.Black),
                Padding = new Thickness(10)
            };




            Button insertRowBtn = new Button { Content = "Insert Row" };
            Button deleteRowBtn = new Button { Content = "Delete Row" };
            Button insertColBtn = new Button { Content = "Insert Col" };
            Button deleteColBtn = new Button { Content = "Delete Col" };
            Button undoBtn = new Button { Content = "Undo" };
            Button redoBtn = new Button { Content = "Redo" };

            insertRowBtn.Click += (s, e) =>  HandleInsertRow();
            deleteRowBtn.Click += (s, e) => HandleDeleteRow();
            insertColBtn.Click += (s, e) => HandleInsertColumn();
            deleteColBtn.Click += (s, e) => HandleDeleteColumn();
            undoBtn.Click += (s, e) => HandleUndo();
            redoBtn.Click += (s, e) => HandleRedo();

            panel.Children.Add(indexBox);
            panel.Children.Add(insertRowBtn);
            panel.Children.Add(deleteRowBtn);
            panel.Children.Add(insertColBtn);
            panel.Children.Add(deleteColBtn);
            panel.Children.Add(undoBtn);
            panel.Children.Add(redoBtn);

            Microsoft.UI.Xaml.Controls.Grid.SetRow(panel, 0);
            rootGrid.Children.Add(panel);
        }


        private void HandleInsertRow()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                ShowError("Please enter a valid number");
                return;
            }

            if (index < 0 || index > viewModel.TotalRows)
            {
                ShowError($"Invalid row index. Must be between 0 and {viewModel.TotalRows}");
                return;
            }

            editor.CommitEdit();
            viewModel.InsertRow(index);

            
        }


        private void HandleDeleteRow()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                ShowError("Please enter a valid number");
                return;
            }

            if (index < 0 || index >= viewModel.TotalRows)
            {
                ShowError($"Invalid row index. Must be between 0 and {viewModel.TotalRows}");
                return;
            }

            if(viewModel.TotalRows == 0)
            {
                ShowError("Cannot delete from empty grid");
                return;
            }

            editor.CommitEdit();
            viewModel.DeleteRow(index);
        }



        private void HandleInsertColumn()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                ShowError("Please enter a valid number");
                return;
            }

            if (index < 0 || index > viewModel.TotalColumns)
            {
                ShowError($"Invalid col index. Must be between 0 and {viewModel.TotalColumns}");
                return;
            }

            editor.CommitEdit();
            viewModel.InsertColumn(index);
        }



        private void HandleDeleteColumn()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                ShowError("Please enter a valid number");
                return;
            }

            if (index < 0 || index >= viewModel.TotalColumns)
            {
                ShowError($"Invalid col index. Must be between 0 and {viewModel.TotalColumns}");
                return;
            }

            if(viewModel.TotalColumns == 0)
            {
                ShowError("Cannot delete from empty grid");
                return;
            }

            editor.CommitEdit();
            viewModel.DeleteColumn(index);
        }

        private void HandleUndo()
        {
            editor.CommitEdit();
            viewModel.Undo();
        }

        private void HandleRedo()
        {
            editor.CommitEdit();
            viewModel.Redo();
        }

        private async void ShowError(string message)
        {
            var xamlRoot = rootGrid.XamlRoot;
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "Ok",
                XamlRoot = xamlRoot
            };
            await dialog.ShowAsync();
        }

    }
}
