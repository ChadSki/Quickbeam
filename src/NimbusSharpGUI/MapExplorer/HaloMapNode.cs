using NimbusSharp;
using System.Collections.Generic;

namespace NimbusSharpGUI.MapExplorer
{
    public class HaloMapNode : ExplorerNode
    {
        private HaloMap map;

        public HaloMapNode(HaloMap map)
        {
            this.map = map;
            var tagsByClass = new Dictionary<string, ExplorerNode>();
            foreach (var tag in map.Tags())
            {
                var tagNode = new HaloTagNode(tag);
                dynamic header = tag.Header;
                var tagClass = header.first_class;
            }
        }

        public override string Name { get { return map.ToString(); } }
    }
}
