using ICSharpCode.TreeView;
using PythonBinding;
using System.Collections.Generic;
using System.Linq;

namespace HalolibWrapper
{
    public class HaloMapNode : SharpTreeNode
    {
        public override object Text { get { return "Halo Map"; } }

        private PyObj HaloMap { get; set; }

        private Dictionary<string, List<HaloTagNode>> TagsByClass { get; set; }

        public HaloMapNode(PyObj map)
        {
            LazyLoading = true;
            HaloMap = map;

            // Load tags
            TagsByClass = new Dictionary<string, List<HaloTagNode>>();
            var tagsIter = HaloMap.CallMethod("tags", null).GetIter();
            var item = tagsIter.Next();
            while (item != null)
            {
                var tag = new HaloTagNode(item);
                var tagClassStr = tag.FirstClass;
                if (!TagsByClass.ContainsKey(tagClassStr))
                {
                    TagsByClass.Add(tagClassStr, new List<HaloTagNode>());
                }
                (TagsByClass[tagClassStr]).Add(tag);
                item = tagsIter.Next();
            }
        }

        public HaloTagNode GetArbitraryTag()
        {
            return new HaloTagNode(
                HaloMap.CallMethod("tag",
                    PyObj.PackTuple1(PyObj.FromStr("ant!"))));
        }

        override protected void LoadChildren()
        {
            /*
            var sortedClasses = Enumerable.OrderBy(TagsByClass.Keys, x => x);
            foreach (var tagClass in sortedClasses)
            {
                var node = new HaloTagClassNode(tagClass, TagsByClass[tagClass]);
                Children.Add(node);
            }
            */

            var tagsIter = HaloMap.CallMethod("tags", null).GetIter();
            var item = tagsIter.Next();
            if (item != null)
            {
                var tag = new HaloTagNode(item);
                Children.Add(tag);
                item = tagsIter.Next();
            }
        }
    }
}
