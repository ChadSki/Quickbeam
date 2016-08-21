using ICSharpCode.TreeView;
using NimbusSharp;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloFieldNode : SharpTreeNode
    {
        private HaloField hfield;
        private string name;

        public HaloFieldNode(HaloStruct hstruct, string name)
        {
            this.name = name;
            hfield = hstruct[name];
        }

        public override object Text { get { return name; } }

        public string TypeName
        {
            get
            {
                return hfield.TypeName;
            }
        }

        public dynamic Value
        {
            get
            {
                return hfield.Value;
            }
        }
    }
}
