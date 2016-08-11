using System;
using PythonBinding;
using System.Collections.Generic;

namespace NimbusSharp
{
    public class HaloTag
    {
        private PyObj pyTag;

        public HaloTag(PyObj pyTag)
        {
            this.pyTag = pyTag;
        }

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }

    public class HaloMap
    {
        private PyObj pyMap;

        public HaloMap(PyObj pyMap)
        {
            this.pyMap = pyMap;
        }

        public IEnumerable<HaloTag> Tags()
        {
            var tagsIter = pyMap.Attr("tags").Call().GetIter();
            PyObj currObj = tagsIter.Next();
            while (currObj != null)
            {
                yield return new HaloTag(currObj);
                currObj = tagsIter.Next();
            }
        }

        public override string ToString()
        {
            return pyMap.ToString();
        }
    }
}
