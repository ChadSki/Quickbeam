using ICSharpCode.TreeView;
using CrappyCppBinding;

namespace HalolibWrapper
{
    public class HaloTagNode : SharpTreeNode
    {
        private PyObj HaloTag { get; set; }

        public HaloTagNode(PyObj tag)
        {
            HaloTag = tag;
            Header = new HaloStructViewModel(tag.GetAttrString("header"));
            Data = new HaloStructViewModel(tag.GetAttrString("data"));
        }

        public string FirstClass
        {
            get { return HaloTag.GetAttrString("first_class").AsStr(); }
        }

        public override object Text
        {
            get { return "tag"; /* HaloTag.GetAttrString("name").AsStr();*/ }
        }

        public override string ToString()
        {
            return "a tag.";
        }

        public HaloStructViewModel Header { get; private set; }
        public HaloStructViewModel Data { get; private set; }
    }
}
