using PythonBinding;

namespace NimbusSharp
{
    public class HaloTag
    {
        private PyObj pyTag;
        public HaloMap ParentMap { get; private set; }

        public HaloTag(PyObj pyTag, HaloMap hMap)
        {
            this.pyTag = pyTag;
            this.ParentMap = hMap;
        }

        private HaloStruct header = null;
        public HaloStruct Header
        {
            get
            {
                if (header == null)
                    header = new HaloStruct(pyTag["header"], this);
                return header;
            }
        }

        private HaloStruct tagData = null;
        public HaloStruct TagData
        {
            get
            {
                if (tagData == null)
                    tagData = new HaloStruct(pyTag["data"], this);
                return tagData;
            }
        }

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }
}
