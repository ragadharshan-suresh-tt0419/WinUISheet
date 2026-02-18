using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using Windows.Foundation;
using gridLevel2LL.Model__Backend_;
using gridLevel2LL.ViewModel;
using gridLevel2LL.View_UI_;

namespace gridLevel2LL
{
    public sealed partial class MainWindow : Window
    {
        private Grid backendGrid;

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

        

        private void InitializeComponents()
        {
            backendGrid = new Grid(20, 20);
            viewModel = new GridViewModel(backendGrid);

            renderer = new GridRenderer(RootGrid, 40, 100, viewModel);

            editor = new CellEditor(renderer, viewModel, ((GridRenderer)renderer).canvas);

            controlPanel = new ControlPanel(RootGrid, viewModel, editor);

            renderer.RenderAll();
        }

        private void RootGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            bool ctrl = Microsoft.UI.Input.InputKeyboardSource
                .GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control)
                .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            if ((!ctrl))
            {
                return;
            }

            if(e.Key == Windows.System.VirtualKey.Z)
            {
                editor.CommitEdit();
                viewModel.Undo();
            }
            else if(e.Key == Windows.System.VirtualKey.Y)
            {
                editor.CommitEdit();
                viewModel.Redo();
            }
        }
    }
}