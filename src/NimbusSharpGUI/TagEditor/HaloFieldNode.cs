using ICSharpCode.TreeView;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloFieldNode : SharpTreeNode
    {

        public HaloFieldNode(string name)
        {
            Name = name;
        }

        public override object Text { get { return Name; } }

        public string Name { get; private set; }
    }
}
