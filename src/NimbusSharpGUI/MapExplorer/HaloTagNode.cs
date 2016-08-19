using NimbusSharp;

namespace NimbusSharpGUI.MapExplorer
{
    internal class HaloTagNode : ExplorerNode
    {
        private HaloTag tag;

        public HaloTagNode(HaloTag tag)
        {
            this.tag = tag;
        }

        public override string Name { get { return tag.ToString(); } }
    }
}