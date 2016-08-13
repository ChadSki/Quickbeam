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

        public HaloStruct Header
        {
            get { return new HaloStruct(pyTag.Attr("header")); }
        }

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }
}
