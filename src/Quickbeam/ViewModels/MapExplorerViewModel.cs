using ICSharpCode.TreeView;
using NimbusSharpGUI.MapExplorer;
using System.Collections.ObjectModel;

namespace Quickbeam.ViewModels
{
    public class MapExplorerViewModel
    {
        private ObservableCollection<ExplorerNode> rootNode;

        public MapExplorerViewModel()
        {
            rootNode = new ObservableCollection<ExplorerNode>();
            rootNode.Add(WorkbenchNode.Instance);
        }

        public ObservableCollection<ExplorerNode> RootNode { get { return rootNode; } }
    }
}
