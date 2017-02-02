using PythonBinding;

namespace NimbusSharp
{
    public class HaloTag
    {
        private PyObj pyTag;
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
            get
            {
                StringField x = Header["first_class"];
                return x.Value;
            }
        }

        public long Ident
        {
            get
            {
                IntField x = Header["ident"];
                return x.Value;
            }
        }

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }
}
