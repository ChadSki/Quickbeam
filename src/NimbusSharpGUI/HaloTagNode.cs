using ICSharpCode.TreeView;
using NimbusSharp;

namespace NimbusSharpGUI
{
    public class HaloTagNode : SharpTreeNode
    {
        private HaloTag tag;

        public HaloTagNode(HaloTag tag)
        {
            this.tag = tag;
        }

        public override object Text { get { return tag.ToString(); } }

        public string Name { get { return "Tag"; } }
    }
}
