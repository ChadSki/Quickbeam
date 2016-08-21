using PythonBinding;

namespace NimbusSharp
{
    public class HaloTag
    {
        private PyObj pyTag;
        public HaloTag(PyObj pyTag)
        {
            this.pyTag = pyTag;
        }

        private HaloStruct header = null;
        public HaloStruct Header
        {
            get
            {
                if (header == null)
                    header = new HaloStruct(pyTag["header"]);
                return header;
            }
        }

        private HaloStruct tagData = null;
        public HaloStruct TagData
        {
            get
            {
                return new HaloStruct(pyTag["data"]);
            }
        }

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }
}
