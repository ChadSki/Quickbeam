using PythonBinding;
using System;
using System.Collections.Generic;

namespace NimbusSharp
{
    public class HaloField { }

    public class HaloStruct
    {
        public static readonly Dictionary<string, Type> dict = new Dictionary<string, Type>
        {
            ["ascii"] = typeof(string),
            ["asciiz"] = typeof(string),
            ["rawdata"] = typeof(string),
            ["enum16"] = typeof(string), // TODO representation of this?
            ["flag"] = typeof(bool),
            ["float32"] = typeof(double),
            ["float64"] = typeof(double),
            ["int8"] = typeof(sbyte),
            ["int16"] = typeof(short),
            ["int32"] = typeof(int),
            ["int64"] = typeof(long),
            ["uint8"] = typeof(byte),
            ["uint16"] = typeof(ushort),
            ["uint32"] = typeof(uint),
            ["uint64"] = typeof(ulong),
        };

        private PyObj pyStruct;
        private List<HaloField> fieldsInOrder = new List<HaloField>();
        private Dictionary<Type, HaloField> fieldsByType = new Dictionary<Type, HaloField>();

        public HaloStruct(PyObj pyStruct)
        {
            this.pyStruct = pyStruct;

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
