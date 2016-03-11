using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quickbeam.Helpers
{
    public class TagHierarchyNode
    {
        public TagHierarchyNode()
        {
            _children = new ObservableCollection<TagHierarchyNode>();
        }

        public TagHierarchyNode(IEnumerable<TagHierarchyNode> children)
        {
            _children = new ObservableCollection<TagHierarchyNode>(children);
        }

        private ObservableCollection<TagHierarchyNode> _children;
        public ObservableCollection<TagHierarchyNode> Children { get { return _children; } }

        public string Name { get { return "tag"; } }

        public string Suffix { get { return "foo"; } }

        public bool IsFolder { get { return _children.Count > 0; } }
    }
}
