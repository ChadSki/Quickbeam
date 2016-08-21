using ICSharpCode.TreeView;
using NimbusSharp;
using PythonBinding;
using System;
using System.Linq;
using System.Collections.Generic;

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
            if (TypeName == "structarray")
            {
                var childStructs = ((IEnumerable<PyObj>)((dynamic)hfield).Value).ToArray();
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
                if (TypeName == "structarray")
                    return label;
                else
                    return ((dynamic)hfield).Value;
            }
        }
    }
}
