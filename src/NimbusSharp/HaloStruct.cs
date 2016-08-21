using PythonBinding;
using System.Linq;
using System.Collections.Generic;

namespace NimbusSharp
{


    public class HaloStruct
    {
        private PyObj pyStruct;
        private IEnumerable<HaloField> fieldsInOrder;
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
                var field = new HaloField(currPair, pyStruct);
                fieldsTemp.Add(field);
                currPair = itemsIter.Next();
            }
            fieldsInOrder = fieldsTemp.OrderBy((hfield) => hfield.Offset);
            foreach (var hfield in fieldsInOrder)
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
