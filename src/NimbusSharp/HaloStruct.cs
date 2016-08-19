using PythonBinding;
using System;
using System.Collections.Generic;

// Mapping of string typenames to a function which does the appropriate cast.
using CastDict = System.Collections.Generic.Dictionary<string, System.Func<object, dynamic>>;

namespace NimbusSharp
{
    public class HaloField { }

    public class HaloStruct
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
        private List<HaloField> fieldsInOrder = new List<HaloField>();
        private Dictionary<Type, HaloField> fieldsByType = new Dictionary<Type, HaloField>();

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;

            Func<object, dynamic> foo = ((x) => (string)x);

            // Iterate through the field objects, construct and cache wrappers
            // TODO
        }

        public IEnumerable<string> FieldNames
        {
            get
            {
                var nameIter = pyStruct.Attr("fields").Attr("keys").Call().GetIter();
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
                var fieldsDict = pyStruct.Attr("fields");
                var nameIter = fieldsDict.Attr("keys").Call().GetIter();
                PyObj currName = nameIter.Next();
                while (currName != null)
                {
                    var fieldObj = fieldsDict.GetItem(currName);
                    yield return fieldObj.Attr("typestring").ToString();
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
