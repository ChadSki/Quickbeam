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
            foreach (var field in hstruct.FieldsInOrder)
            {
                Children.Add(HaloFieldNode.Build(hstruct, field));
            }
            IsExpanded = true;
        }

        public override object Text { get { return label; } }

        public string Value { get { return ""; } }
    }
}
