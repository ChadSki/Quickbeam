using System.Collections.ObjectModel;

namespace NimbusSharpGUI.MapExplorer
{
    public class ExplorerNode
    {
        public ObservableCollection<ExplorerNode> Children { get; } = new ObservableCollection<ExplorerNode>();
        public virtual string Name { get; }
    }
}
