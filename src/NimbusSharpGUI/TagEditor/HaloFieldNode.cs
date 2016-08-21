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
        private string name;

        public HaloFieldNode(HaloStruct hstruct, string name)
        {
            this.name = name;
            hfield = hstruct[name];

            // Load children eagerly
            if (TypeName == "structarray")
            {
                IEnumerable<PyObj> structArray = hfield.Value;
                PyObj[] childStructs = structArray.ToArray();
                for (int i = 0; i < childStructs.Length; i++)
                {
                    Children.Add(
                        new HaloStructNode(
                            new HaloStruct(childStructs[i]),
                            string.Format("struct #{0}", i)));
                }
            }
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
                if (TypeName == "structarray")
                    return "";
                else
                    return hfield.Value;
            }
        }
    }
}
