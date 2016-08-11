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

        public override string ToString()
        {
            return pyTag.ToString();
        }
    }
}
