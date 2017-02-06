using PythonBinding;

namespace NimbusSharp
{
    public class HaloTag
    {
        internal PyObj pyTag;
        public HaloMap ParentMap { get; private set; }

        /// Architecturally, this should only be called by HaloMap
        internal HaloTag(PyObj pyTag, HaloMap hMap)
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

        public string FirstClass
        {
            get { return (Header["first_class"] as StringField).Value; }
        }

        public long Ident
        {
            get { return (Header["ident"] as IntField).Value; }
        }

        public string Name
        {
            get { return (Header["name"] as StringField).Value; }
        }

        public string UniqueName
        {
            get { return string.Format("[{0}]{1}", Ident, Name); }
        }

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }
}
