using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using gridLevel2LL.Model;
using gridLevel2LL.ViewModel;
using gridLevel2LL.View;

namespace gridLevel2LL
{
    public sealed partial class MainWindow : Window
    {
        private IGrid backendGrid;
        private IGridRenderer renderer;
        private ICellEditor editor;
        private ControlPanel controlPanel;
        private GridViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeComponents();
            RootGrid.KeyDown += RootGrid_KeyDown;
        }

        private async void InitializeComponents()
        {
            backendGrid = new Grid(20, 20);
            viewModel = new GridViewModel(backendGrid);

            try
            {
                await viewModel.LoadFromApiAsync(gridId: 6);
            }
            catch
            {
                await viewModel.CreateNewGridAsync("My Spreadsheet");
            }

            renderer = new GridRenderer(RootGrid, 40, 100, viewModel);
            editor = new CellEditor(renderer, viewModel, ((GridRenderer)renderer).canvas);
            controlPanel = new ControlPanel(RootGrid, viewModel, editor);

            renderer.RenderAll();
        }

        private async void RootGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            bool ctrl = Microsoft.UI.Input.InputKeyboardSource
                .GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control)
                .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            if (!ctrl) return;

            if (e.Key == Windows.System.VirtualKey.Z)
            {
                editor.CommitEdit();
                await viewModel.Undo();
            }
            else if (e.Key == Windows.System.VirtualKey.Y)
            {
                editor.CommitEdit();
                await viewModel.Redo();
            }
        }
    }
}