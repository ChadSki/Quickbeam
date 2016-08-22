using NimbusSharp;

namespace NimbusSharpGUI.MapExplorer
{
    public class HaloTagNode : ExplorerNode
    {
        public HaloTagNode(HaloTag tag)
        {
            Tag = tag;
        }

        public HaloTag Tag { get; private set; }

        public override string Name { get { return Tag.ToString(); } }
    }
}