using PythonBinding;
using System.Collections.Generic;

namespace NimbusSharp
{
    public class HaloStruct
    {
        private PyObj pyStruct;

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;
        }

        public IEnumerable<string> Fields
        {
            get
            {
                var fieldNameIter = pyStruct
                    .Attr("fields").Attr("keys").Call().GetIter();
                PyObj currName = fieldNameIter.Next();
                while (currName != null)
                {
                    yield return currName.ToString();
                    currName = fieldNameIter.Next();
                }
            }
        }

        public override string ToString()
        {
            return pyStruct.ToString();
        }
    }
}
