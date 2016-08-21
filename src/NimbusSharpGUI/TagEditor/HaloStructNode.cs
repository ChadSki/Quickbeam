using ICSharpCode.TreeView;
using NimbusSharp;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloStructNode : SharpTreeNode
    {
        private HaloStruct hstruct;
        private string label;

        public HaloStructNode(HaloStruct hstruct, string label)
        {
            this.hstruct = hstruct;
            this.label = label;
            LazyLoading = true;
        }

        public override object Text { get { return label; } }

        public string Value { get { return ""; } }

        protected override void LoadChildren()
        {
            foreach (var name in hstruct.FieldNames)
            {
                Children.Add(new HaloFieldNode(hstruct, name));
            }
        }
    }
}
