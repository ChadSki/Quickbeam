using NimbusSharp;
using System.Collections.Generic;
using System.Linq;

namespace NimbusSharpGUI.MapExplorer
{
    public class HaloMapNode : ExplorerNode
    {
        private HaloMap map;

        public HaloMapNode(HaloMap map)
        {
            this.map = map;
            var tagsByClass = new Dictionary<string, List<HaloTagNode>>();
            foreach (var tag in map.Tags())
            {
                dynamic header = tag.Header;
                string tagClass = header.first_class;
                if (!tagsByClass.ContainsKey(tagClass))
                {
                    tagsByClass.Add(tagClass, new List<HaloTagNode>());
                }
                tagsByClass[tagClass].Add(new HaloTagNode(tag));
            }

            foreach (string tagClass in tagsByClass.Keys.OrderBy(x => x))
            {
                Children.Add(new HaloTagClassNode(tagClass, tagsByClass[tagClass]));
            }
        }

        public override string Name { get { return map.ToString(); } }
    }
}
