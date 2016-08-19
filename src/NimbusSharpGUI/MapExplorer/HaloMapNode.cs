using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusSharp;
using System.Collections.ObjectModel;

namespace NimbusSharpGUI.MapExplorer
{
    public class HaloMapNode : ExplorerNode
    {
        private HaloMap map;

        public HaloMapNode(HaloMap map)
        {
            this.map = map;
            foreach (var tag in map.Tags())
            {
                Children.Add(new HaloTagNode(tag));
            }
        }

        public override string Name { get { return map.ToString(); } }
    }
}
