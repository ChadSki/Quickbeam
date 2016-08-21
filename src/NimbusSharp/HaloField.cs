using PythonBinding;
using System;

// Mapping of string typenames to a function which does the appropriate cast.
using CastFn = System.Func<PythonBinding.PyObj, dynamic>;
using CastDict = System.Collections.Generic.Dictionary<string, System.Func<PythonBinding.PyObj, dynamic>>;

namespace NimbusSharp
{
    public class HaloField
    {
        private PyObj pyStruct;
        private static readonly CastDict castTo = new CastDict
        {
            ["ascii"] = ((x) => x.ToString()),
            ["asciiz"] = ((x) => x.ToString()),
            ["rawdata"] = ((x) => x.ToString()),
            ["enum16"] = ((x) => "TODO: enum16"),
            ["flag"] = ((x) => "TODO: flag"),
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

            ["tagreference"] = ((x) => "TODO: tagreference"),
        };

        public HaloField(PyObj fieldTuple, PyObj pyStruct)
        {
            // Unwrap the Tuple[str, Field]
            Name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();

            // Keep the type name so we know what to cast to
            var secondItem = fieldTuple.GetItem(PyObj.FromLong(1));
            TypeName = secondItem["typestring"].ToString();

            this.pyStruct = pyStruct;
        }

        public string TypeName { get; private set; }

        public string Name { get; private set; }

        public dynamic Value
        {
            get
            {
                CastFn castFn;
                if (castTo.TryGetValue(TypeName, out castFn))
                {
                    Console.WriteLine("yay");
                }
                else
                {
                    Console.WriteLine(
                        string.Format("Key = '{0}' is not found.", TypeName));
                }

                var pyVal = pyStruct[Name];
                dynamic result = castFn(pyVal);
                return result;
            }
        }
    }
}
