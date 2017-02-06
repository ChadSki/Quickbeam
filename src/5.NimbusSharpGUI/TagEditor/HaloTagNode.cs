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
        private HaloTag htag;

        public HaloTagNode(HaloTag htag)
        {
            this.htag = htag;
            Children.Add(new HaloStructNode(htag.Header, "header"));
            Children.Add(new HaloStructNode(htag.TagData, "tagdata"));
        }

        public override object Text
        {
            get
            {
                var header = htag.Header;
                return string.Format("[{0}]{1}({2})",
                    header["first_class"].Value,
                    header["name"].Value,
                    header["ident"].Value);
            }
        }
    }
}
