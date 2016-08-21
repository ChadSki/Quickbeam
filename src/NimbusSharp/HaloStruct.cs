using PythonBinding;
using System.Linq;
using System.Collections.Generic;

namespace NimbusSharp
{


    public class HaloStruct
    {
        private PyObj pyStruct;
        private Dictionary<string, HaloField> fieldsByName = new Dictionary<string, HaloField>();

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;

            // Iterate through the field objects
            var fieldsTemp = new List<HaloField>();
            var itemsIter = pyStruct["fields"]["items"].Call().GetIter();
            PyObj currPair = itemsIter.Next();
            while (currPair != null)
            {
                var field = HaloField.Build(currPair, pyStruct);
                fieldsTemp.Add(field);
                currPair = itemsIter.Next();
            }
            FieldsInOrder = fieldsTemp.OrderBy((hfield) => hfield.Offset);
            foreach (var hfield in FieldsInOrder)
            {
                fieldsByName.Add(hfield.Name, hfield);
            }
        }

        public dynamic this[string attrName]
        {
            get
            {
                return fieldsByName[attrName];
            }
        }

        public IEnumerable<HaloField> FieldsInOrder { get; private set; }

        public override string ToString()
        {
            return pyStruct.ToString();
        }
    }
}
