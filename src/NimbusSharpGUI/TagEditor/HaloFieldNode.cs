using ICSharpCode.TreeView;
using NimbusSharp;
using System.Linq;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloFieldNode : SharpTreeNode
    {
        private HaloField hfield;

        public HaloFieldNode(HaloStruct hstruct, HaloField hfield)
        {
            this.hfield = hfield;

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
                if (sa.Children.Count < 2)
                    IsExpanded = true;
            }
        }

        public override object Text { get { return hfield.Name; } }

        public HaloField Field
        {
            get { return hfield; }
        }
    }
}
