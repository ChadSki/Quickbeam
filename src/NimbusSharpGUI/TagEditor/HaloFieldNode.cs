using ICSharpCode.TreeView;
using NimbusSharp;
using System.Linq;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloFieldNode : SharpTreeNode
    {
        private HaloField hfield;
        private string label;

        public HaloFieldNode(HaloStruct hstruct, HaloField hfield)
        {
            this.hfield = hfield;

            // Load children eagerly
            var saf = hfield as StructArrayField;
            if (saf != null)
            {
                var childStructs = saf.Value.ToArray();
                if (childStructs.Length == 0)
                {
                    label = "None";
                }
                else
                {
                    label = "";
                    for (int i = 0; i < childStructs.Length; i++)
                    {
                        Children.Add(
                            new HaloStructNode(
                                new HaloStruct(childStructs[i]),
                                string.Format("[{0}]", i)));
                    }
                    if (childStructs.Length < 2)
                        IsExpanded = true;
                }
            }
        }

        public override object Text { get { return hfield.Name; } }

        public dynamic Value
        {
            get
            {
                if (hfield is StructArrayField)
                    return label;
                else
                    return ((dynamic)hfield).Value;
            }
        }
    }
}
