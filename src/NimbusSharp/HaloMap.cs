using PythonBinding;
using System.Collections.Generic;
using System.Linq;

namespace NimbusSharp
{
    public class HaloMap
    {
        private PyObj pyMap;

        public HaloMap(PyObj pyMap)
        {
            this.pyMap = pyMap;
        }

        public IEnumerable<HaloTag> Tags()
        {
            var tagsIter = pyMap["tags"].Call().GetIter();
            PyObj currObj = tagsIter.Next();
            while (currObj != null)
            {
                yield return new HaloTag(currObj, this);
                currObj = tagsIter.Next();
            }
        }

        private List<string> _tagClasses = null;
        public List<string> TagClasses
        {
            get
            {
                if (_tagClasses == null)
                {
                    // Collect classes
                    var classesTemp = new HashSet<string>();
                    var tagsIter = pyMap["tags"].Call().GetIter();
                    PyObj currTag = tagsIter.Next();
                    while (currTag != null)
                    {
                        classesTemp.Add(currTag["header"]["first_class"].ToString());
                        currTag = tagsIter.Next();
                    }

                    // Sort alphabetically and cache result
                    // TODO: this probably needs to be updated if we add new tags?
                    _tagClasses = classesTemp.OrderBy(x => x).ToList();
                }
                return _tagClasses;
            }
        }

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
