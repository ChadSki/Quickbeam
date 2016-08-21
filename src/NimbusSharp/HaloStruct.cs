using PythonBinding;
using System.Linq;
using System.Collections.Generic;

namespace NimbusSharp
{


    public class HaloStruct
    {
        private PyObj pyStruct;
        private List<HaloField> fieldsInOrder = new List<HaloField>();
        private Dictionary<string, HaloField> fieldsByName = new Dictionary<string, HaloField>();

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;

            // Iterate through the field objects
            var itemsIter = pyStruct["fields"]["items"].Call().GetIter();
            PyObj currPair = itemsIter.Next();
            while (currPair != null)
            {
                var field = new HaloField(currPair, pyStruct);
                fieldsInOrder.Add(field);
                currPair = itemsIter.Next();
            }
            
            foreach (var hfield in fieldsInOrder.OrderBy((hfield) => hfield.Offset))
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

        public IEnumerable<string> FieldNames
        {
            get
            {
                foreach (var hfield in fieldsInOrder)
                {
                    yield return hfield.Name;
                }
                yield break;
            }
        }

        public IEnumerable<string> FieldTypes
        {
            get
            {
                var fieldsDict = pyStruct["fields"];
                var nameIter = fieldsDict["keys"].Call().GetIter();
                PyObj currName = nameIter.Next();
                while (currName != null)
                {
                    var fieldObj = fieldsDict.GetItem(currName);
                    yield return fieldObj["typestring"].ToString();
                    currName = nameIter.Next();
                }
            }
        }

        public override string ToString()
        {
            return pyStruct.ToString();
        }
    }
}
