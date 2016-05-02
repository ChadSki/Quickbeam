using ICSharpCode.TreeView;
using System.Collections.Generic;

namespace HalolibWrapper
{
    class HaloTagClassNode : SharpTreeNode
    {
        private List<HaloTagNode> Tags { get; set; }
        private string ClassName { get; set; }
        public override object Text { get { return ClassName; } }

        public HaloTagClassNode(string className, List<HaloTagNode> tags)
        {
            ClassName = className;
            Tags = tags;
        }

        override protected void LoadChildren()
        {
            foreach(var tag in Tags)
            {
                Children.Add(tag);
            }
        }
    }
}
