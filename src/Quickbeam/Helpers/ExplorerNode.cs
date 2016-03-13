using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quickbeam.Helpers
{
    public class ExplorerNode
    {
        public ExplorerNode()
        {
            children = new ObservableCollection<ExplorerNode>();
        }

        public ExplorerNode(IEnumerable<ExplorerNode> chld)
        {
            children = new ObservableCollection<ExplorerNode>(chld);
        }

        private ObservableCollection<ExplorerNode> children;
        public ObservableCollection<ExplorerNode> Children { get { return children; } }

        public string Name { get { return "tag"; } }

        public string Suffix { get { return "foo"; } }

        public bool IsFolder { get { return Children.Count > 0; } }
    }
}
