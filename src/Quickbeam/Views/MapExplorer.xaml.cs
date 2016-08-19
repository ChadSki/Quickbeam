using Quickbeam.ViewModels;
using System.Windows.Controls;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for MapExplorer.xaml
    /// </summary>
    public partial class MapExplorer : UserControl
    {
        public MapExplorerViewModel ViewModel { get; private set; }

        public MapExplorer()
        {
            InitializeComponent();
            DataContext = ViewModel = new MapExplorerViewModel();
        }
    }
}
