using gridLevel2LL.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gridLevel2LL.View;
using gridLevel2LL.Model;

namespace gridLevel2LL
{
    internal class ControlPanel
    {
        Microsoft.UI.Xaml.Controls.Grid rootGrid;
        private GridViewModel viewModel;
        private ICellEditor editor;
        private TextBox indexBox;

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

            insertRowBtn.Click += async (s, e) => await HandleInsertRow();
            deleteRowBtn.Click += async (s, e) => await HandleDeleteRow();
            insertColBtn.Click += async (s, e) => await HandleInsertColumn();
            deleteColBtn.Click += async (s, e) => await HandleDeleteColumn();
            undoBtn.Click += async (s, e) => await HandleUndo();
            redoBtn.Click += async (s, e) => await HandleRedo();

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

        private async Task HandleInsertRow()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                await ShowError("Please enter a valid number");
                return;
            }

            if (index < 0 || index > viewModel.TotalRows)
            {
                await ShowError($"Invalid row index. Must be between 0 and {viewModel.TotalRows}");
                return;
            }

            editor.CommitEdit();
            try
            {
                await viewModel.InsertRow(index);
            }
            catch (Exception ex)
            {
                await ShowError($"Insert row failed: {ex.Message}");
            }
        }

        private async Task HandleDeleteRow()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                await ShowError("Please enter a valid number");
                return;
            }

            if (viewModel.TotalRows == 0)
            {
                await ShowError("Cannot delete from empty grid");
                return;
            }

            if (index < 0 || index >= viewModel.TotalRows)
            {
                await ShowError($"Invalid row index. Must be between 0 and {viewModel.TotalRows - 1}");
                return;
            }

            editor.CommitEdit();
            try
            {
                await viewModel.DeleteRow(index);
            }
            catch (Exception ex)
            {
                await ShowError($"Delete row failed: {ex.Message}");
            }
        }

        private async Task HandleInsertColumn()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                await ShowError("Please enter a valid number");
                return;
            }

            if (index < 0 || index > viewModel.TotalColumns)
            {
                await ShowError($"Invalid col index. Must be between 0 and {viewModel.TotalColumns}");
                return;
            }

            editor.CommitEdit();
            try
            {
                await viewModel.InsertColumn(index);
            }
            catch (Exception ex)
            {
                await ShowError($"Insert column failed: {ex.Message}");
            }
        }

        private async Task HandleDeleteColumn()
        {
            if (!int.TryParse(indexBox.Text, out int index))
            {
                await ShowError("Please enter a valid number");
                return;
            }

            if (viewModel.TotalColumns == 0)
            {
                await ShowError("Cannot delete from empty grid");
                return;
            }

            if (index < 0 || index >= viewModel.TotalColumns)
            {
                await ShowError($"Invalid col index. Must be between 0 and {viewModel.TotalColumns - 1}");
                return;
            }

            editor.CommitEdit();
            try
            {
                await viewModel.DeleteColumn(index);
            }
            catch (Exception ex)
            {
                await ShowError($"Delete column failed: {ex.Message}");
            }
        }

        private async Task HandleUndo()
        {
            editor.CommitEdit();
            try
            {
                await viewModel.Undo();
            }
            catch (Exception ex)
            {
                await ShowError($"Undo failed: {ex.Message}");
            }
        }

        private async Task HandleRedo()
        {
            editor.CommitEdit();
            try
            {
                await viewModel.Redo();
            }
            catch (Exception ex)
            {
                await ShowError($"Redo failed: {ex.Message}");
            }
        }

        private async Task ShowError(string message)
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