using System.Collections.Generic;

namespace NimbusSharpGUI.MapExplorer
{
    public class HaloTagClassNode : ExplorerNode
    {
        private string tagClass;

        public HaloTagClassNode(string tagClass, List<HaloTagNode> list)
        {
            this.tagClass = tagClass;
            foreach (var node in list)
            {
                Children.Add(node);
            }
        }

        public override string Name { get { return tagClass; } }
    }
}
