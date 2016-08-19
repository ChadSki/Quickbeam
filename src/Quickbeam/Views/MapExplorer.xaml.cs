using NimbusSharpGUI.MapExplorer;
using Quickbeam.ViewModels;
using System.Windows.Controls;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for MapExplorer.xaml
    /// </summary>
    public partial class MapExplorer : UserControl
    {
        public MapExplorer()
        {
            InitializeComponent();
            DataContext = WorkbenchNode.Instance;
        }
    }
}
