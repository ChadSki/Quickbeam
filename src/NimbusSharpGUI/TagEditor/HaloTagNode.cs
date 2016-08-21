using ICSharpCode.TreeView;
using NimbusSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloTagNode : SharpTreeNode
    {
        public HaloTagNode(HaloTag htag)
        {
            Children.Add(new HaloStructNode(htag.Header, "header"));
            Children.Add(new HaloStructNode(htag.TagData, "tagdata"));
        }
    }
}
