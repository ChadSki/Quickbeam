using ICSharpCode.TreeView;
using NimbusSharp;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloFieldNode : SharpTreeNode
    {
        public HaloFieldNode(HaloStruct hstruct, HaloField hfield)
        {
            Field = hfield;

            // Load children eagerly
            var sa = hfield as StructArrayField;
            if (sa != null)
            {
                for (int i = 0; i < sa.Children.Count; i++)
                {
                    Children.Add(
                        new HaloStructNode(
                            new HaloStruct(sa.Children[i]),
                            string.Format("[{0}]", i)));
                }
                // Auto-expand if there aren't multiple children
                if (sa.Children.Count < 2)
                    IsExpanded = true;
            }
        }

        public override object Text { get { return Field.Name; } }

        public HaloField Field { get; private set; }
    }
}
