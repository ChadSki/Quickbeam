using PythonBinding;
using System.Collections.Generic;

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
                yield return new HaloTag(currObj);
                currObj = tagsIter.Next();
            }
        }

        public HaloTag ArbitraryTag
        {
            get
            {
                var tagsIter = pyMap["tags"].Call("bipd", "").GetIter();
                PyObj currObj = tagsIter.Next();
                return new HaloTag(currObj);
            }
        }

        public override string ToString()
        {
            return pyMap.ToString();
        }
    }
}
