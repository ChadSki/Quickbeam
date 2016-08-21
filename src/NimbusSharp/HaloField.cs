using PythonBinding;
using System;

// Mapping of string typenames to a function which does the appropriate cast.
using CastFn = System.Func<PythonBinding.PyObj, dynamic>;
using CastDict = System.Collections.Generic.Dictionary<string, System.Func<PythonBinding.PyObj, dynamic>>;
using System.Collections.Generic;

namespace NimbusSharp
{
    public class HaloField
    {
        private PyObj pyStruct;
        private PyObj pyField;

        private static IEnumerable<PyObj> castStructArray(PyObj pyStructArray)
        {
            var iter = pyStructArray.GetIter();
            var currStruct = iter.Next();
            while (currStruct != null)
            {
                yield return currStruct;
                currStruct = iter.Next();
            }
            yield break;
        }

        private static readonly CastDict castTo = new CastDict
        {
            // basic fields
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

            // Halo fields
            ["asciizptr"] = ((x) => x.ToString()),
            ["tagreference"] = ((x) => x.ToString()),
            ["structarray"] = castStructArray,
        };

        public static HaloField Build(PyObj fieldTuple, PyObj pyStruct)
        {
            return new HaloField(fieldTuple, pyStruct);
        }

        private HaloField(PyObj fieldTuple, PyObj pyStruct)
        {
            // Unwrap the Tuple[str, Field]
            Name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();
            pyField = fieldTuple.GetItem(PyObj.FromLong(1));

            this.pyStruct = pyStruct;
        }

        public string TypeName { get { return pyField["typestring"].ToString(); } }

        public string Name { get; private set; }

        public long Offset
        {
            get
            {
                return pyField["offset"].ToLong();
            }
        }

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
