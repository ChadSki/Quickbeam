using ICSharpCode.TreeView;
using NimbusSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusSharpGUI
{
    public class HaloMapNode : SharpTreeNode
    {
        private HaloMap map;

        public HaloMapNode(HaloMap map)
        {
            this.map = map;
        }
    }
}
