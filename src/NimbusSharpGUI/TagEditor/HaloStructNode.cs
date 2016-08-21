using ICSharpCode.TreeView;
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
