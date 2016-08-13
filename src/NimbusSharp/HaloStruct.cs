using PythonBinding;

namespace NimbusSharp
{
    public class HaloStruct
    {
        private PyObj pyStruct;

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;
        }



        public override string ToString()
        {
            return pyStruct.ToString();
        }
    }
}
