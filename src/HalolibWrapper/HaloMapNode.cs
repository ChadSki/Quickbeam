using ICSharpCode.TreeView;
using PythonBinding;
using System.Collections.Generic;
using System.Linq;

namespace HalolibWrapper
{
    class HaloMapNode : SharpTreeNode
    {
        private PyObj HaloMap { get; set; }

        private Dictionary<string, List<HaloTagNode>> TagsByClass { get; set; }

        public HaloMapNode(PyObj map)
        {
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

        override protected void LoadChildren()
        {
            var sortedClasses = Enumerable.OrderBy(TagsByClass.Keys, x => x);
            foreach (var tagClass in sortedClasses)
            {
                Children.Add(new HaloTagClassNode(tagClass, TagsByClass[tagClass]));
            }
        }
    }
}
