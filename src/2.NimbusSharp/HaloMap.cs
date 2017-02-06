using PythonBinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NimbusSharp
{
    public class HaloMap
    {
        private PyObj pyMap;
        private List<HaloTag> tagsInOrder = new List<HaloTag>();
        private Dictionary<string, List<HaloTag>> tagsByClass = new Dictionary<string, List<HaloTag>>();

        public HaloMap(PyObj pyMap)
        {
            this.pyMap = pyMap;
            tagsFromPython().ToList().ForEach(BuildAndRegisterTag);

            // TODO: Load tags later, too?
            // I suppose we shouldn't expect anything to change unless we change it...
        }

        /// Iterate through all tags via the PythonBinding layer.
        private IEnumerable<PyObj> tagsFromPython()
        {
            var tagsIter = pyMap["tags"].Call().GetIter();
            PyObj currObj = tagsIter.Next();
            while (currObj != null)
            {
                yield return currObj;
                currObj = tagsIter.Next();
            }
        }

        /// Verify whether our tag collection is up to date
        private bool IsTagCollectionValid()
        {
            var pyTags = tagsFromPython().GetEnumerator();
            return tagsInOrder.All(
                tag => (tag.Ident == pyTags.Current["ident"].ToLong())
                    && (pyTags.MoveNext()));
        }

        /// Create HaloTag object and save references to it,
        /// so we're always talking about the same thing.
        private void BuildAndRegisterTag(PyObj pyTag)
        {
            var tag = new HaloTag(pyTag, this);
            tagsInOrder.Add(tag);
            var firstClass = tag.FirstClass;
            if (! tagsByClass.ContainsKey(firstClass))
            {
                tagsByClass[firstClass] = new List<HaloTag>();
            }
            tagsByClass[firstClass].Add(tag);
        }

        /// HaloTags in the same order the appear in the map.
        public IEnumerable<HaloTag> Tags
        {
            get { return tagsInOrder; }
        }

        /// Four-character tag classes, in alphabetical order.
        public IEnumerable<string> TagClasses
        {
            get { return tagsByClass.Keys.OrderBy(x => x); }
        }

        public IEnumerable<HaloTag> TagsOfClass(string firstClass)
        {
            if (firstClass == "")
            {
                // TODO do I need a None tag?
                return new List<HaloTag>();
            }
            return tagsByClass[firstClass];
        }

        public HaloTag TagFromIdent(long targetIdent)
        {
            return tagsInOrder.First(tag => tag.Ident == targetIdent);
        }

        public HaloTag TagFromPyObj(PyObj pyTag)
        {
            var x = pyTag;
            var y = x["header"];
            var z = y["ident"];
            var targetIdent = z.ToLong();
            return TagFromIdent(targetIdent);
        }

        public HaloTag TagFromUniqueName(string name)
        {
            var identMatch = Regex.Match(name, @"\[(.+)\]");
            return TagFromIdent(long.Parse(identMatch.Groups[1].Value));
        }

        /// Get just one tag.
        /// Useful for testing purposes while hacking on the lower layers.
        public HaloTag ArbitraryTag
        {
            get
            {
                var tagsIter = pyMap["tags"].Call("bipd", "").GetIter();
                PyObj currObj = tagsIter.Next();
                return new HaloTag(currObj, this);
            }
        }

        public override string ToString()
        {
            return pyMap.ToString();
        }
    }
}
