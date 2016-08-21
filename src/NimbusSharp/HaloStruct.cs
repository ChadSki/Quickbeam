using PythonBinding;
using System;
using System.Collections.Generic;
using System.Dynamic;

// Mapping of string typenames to a function which does the appropriate cast.
using CastDict = System.Collections.Generic.Dictionary<string, System.Func<PythonBinding.PyObj, dynamic>>;

namespace NimbusSharp
{
    public class HaloField
    {
        private PyObj pyStruct;
        private string typeName;
        private static readonly CastDict castTo = new CastDict
        {
            ["ascii"] = ((x) => x.ToString()),
            ["asciiz"] = ((x) => x.ToString()),
            ["rawdata"] = ((x) => x.ToString()),
            ["enum16"] = ((x) => { throw new NotImplementedException(); }),
            ["flag"] = ((x) => { throw new NotImplementedException(); }),
            ["float32"] = ((x) => x.ToDouble()),
            ["float64"] = ((x) => x.ToDouble()),
            ["int8"] = ((x) => (sbyte)x.ToLong()),
            ["int16"] = ((x) => (short)x.ToLong()),
            ["int32"] = ((x) => (int)x.ToLong()),
            ["int64"] = ((x) => x.ToLong()),
            ["uint8"] = ((x) => (byte)x.ToLong()),
            ["uint16"] = ((x) => (ushort)x.ToLong()),
            ["uint32"] = ((x) => (uint)x.ToLong()),
            ["uint64"] = ((x) => (ulong)x.ToLong()),
        };

        public HaloField(PyObj fieldTuple, PyObj pyStruct)
        {
            // Unwrap the Tuple[str, Field]
            Name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();

            // Keep the type name so we know what to cast to
            var secondItem = fieldTuple.GetItem(PyObj.FromLong(1));
            typeName = secondItem["typestring"].ToString();

            this.pyStruct = pyStruct;
        }

        public string Name { get; private set; }

        public dynamic Value
        {
            get
            {
                return castTo[typeName](pyStruct[Name]);
            }
        }
    }

    public class HaloStruct
    {
        private PyObj pyStruct;
        private List<HaloField> fieldsInOrder = new List<HaloField>();
        private Dictionary<string, HaloField> fieldsByName = new Dictionary<string, HaloField>();

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;

            // Iterate through the field objects, caching for later access
            var itemsIter = pyStruct["fields"]["items"].Call().GetIter();
            PyObj currPair = itemsIter.Next();
            while (currPair != null)
            {
                var field = new HaloField(currPair, pyStruct);
                fieldsInOrder.Add(field);
                fieldsByName.Add(field.Name, field);
                currPair = itemsIter.Next();
            }
        }

        public dynamic this[string attrName]
        {
            get
            {
                return fieldsByName[attrName].Value;
            }
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
