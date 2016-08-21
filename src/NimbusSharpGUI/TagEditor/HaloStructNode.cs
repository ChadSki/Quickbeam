using ICSharpCode.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusSharp;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloStructNode : SharpTreeNode
    {
        private HaloStruct hstruct;

        public HaloStructNode(HaloStruct hstruct)
        {
            this.hstruct = hstruct;
            LazyLoading = true;
        }

        public override object Text { get { return "Struct"; } }

        public string Name { get { return hstruct.ToString(); } }

        protected override void LoadChildren()
        {
            foreach (var name in hstruct.FieldNames)
            {
                Children.Add(new HaloFieldNode(name));
            }
        }
    }
}
