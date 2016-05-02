using ICSharpCode.TreeView;
using PythonBinding;

namespace HalolibWrapper
{
    class HaloTagNode : SharpTreeNode
    {
        private PyObj HaloTag { get; set; }

        public HaloTagNode(PyObj tag)
        {
            HaloTag = tag;
        }

        public string FirstClass
        {
            get { return HaloTag.GetAttrString("first_class").AsStr(); }
        }
        public override object Text
        {
            get { return HaloTag.GetAttrString("name").AsStr(); }
        }
    }
}
