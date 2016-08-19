using PythonBinding;
using System;
using System.Collections.Generic;
using System.Dynamic;

// Mapping of string typenames to a function which does the appropriate cast.
using CastDict = System.Collections.Generic.Dictionary<string, System.Func<object, dynamic>>;

namespace NimbusSharp
{
    public class HaloField { }

    public class HaloStruct : DynamicObject
    {
        public static readonly CastDict castDict = new CastDict
        {
            ["ascii"] = ((x) => (string)x),
            ["asciiz"] = ((x) => (string)x),
            ["rawdata"] = ((x) => (string)x),
            ["enum16"] = ((x) => (string)x), // TODO representation of this?
            ["flag"] = ((x) => (bool)x),
            ["float32"] = ((x) => (double)x),
            ["float64"] = ((x) => (double)x),
            ["int8"] = ((x) => (sbyte)x),
            ["int16"] = ((x) => (short)x),
            ["int32"] = ((x) => (int)x),
            ["int64"] = ((x) => (long)x),
            ["uint8"] = ((x) => (byte)x),
            ["uint16"] = ((x) => (ushort)x),
            ["uint32"] = ((x) => (uint)x),
            ["uint64"] = ((x) => (ulong)x),
        };

        private PyObj pyStruct;
        private Dictionary<string, PyObj> fieldsByName = new Dictionary<string, PyObj>();
        private List<PyObj> fieldsInOrder = new List<PyObj>();

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;

            // Iterate through the field objects, caching for later access
            var itemsIter = pyStruct["fields"]["items"].Call().GetIter();
            PyObj currPair = itemsIter.Next();
            while (currPair != null)
            {
                var fieldName = currPair;
                var fieldObj = currPair;
                fieldsInOrder.Add(fieldObj);
                fieldsByName.Add(fieldName.ToString(), fieldObj);
                currPair = itemsIter.Next();
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PyObj field;
            bool didItWork = fieldsByName.TryGetValue(binder.Name, out field);
            result = field;
            return didItWork;
        }

        public IEnumerable<string> FieldNames
        {
            get
            {
                var nameIter = pyStruct["fields"]["keys"].Call().GetIter();
                PyObj currName = nameIter.Next();
                while (currName != null)
                {
                    yield return currName.ToString();
                    currName = nameIter.Next();
                }
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
