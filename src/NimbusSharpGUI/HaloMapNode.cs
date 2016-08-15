using ICSharpCode.TreeView;
using NimbusSharp;

namespace NimbusSharpGUI
{
    public class HaloMapNode : SharpTreeNode
    {
        private HaloMap map;

        public override object Text { get { return "Map"; } }

        public string Name { get { return "Bar"; } }

        public HaloMapNode(HaloMap map)
        {
            this.map = map;
            LazyLoading = true;
        }

        protected override void LoadChildren()
        {
            foreach (var tag in map.Tags())
            {
                Children.Add(new HaloTagNode(tag));
            }
        }
    }
}
