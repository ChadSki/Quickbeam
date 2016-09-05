using PythonBinding;
using System;

// Constructor type signature
using ConstructFn = System.Func<
        PythonBinding.PyObj,
        PythonBinding.PyObj,
        NimbusSharp.HaloField>;

using System.Collections.Generic;

namespace NimbusSharp
{
    public class HaloField
    {
        protected PyObj pyStruct;
        protected PyObj pyField;

        protected HaloField(PyObj fieldTuple, PyObj pyStruct)
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

        private static readonly Dictionary<string, ConstructFn> constructorDict = new Dictionary<string, ConstructFn>
        {
            // basic fields
            ["ascii"] = ((fTup, fStruct) => new StringField(fTup, fStruct)),
            ["asciiz"] = ((fTup, fStruct) => new StringField(fTup, fStruct)),
            ["rawdata"] = ((fTup, fStruct) => new StringField(fTup, fStruct)),
            ["enum16"] = ((fTup, fStruct) => new EnumField(fTup, fStruct)),
            ["flag"] = ((fTup, fStruct) => new FlagField(fTup, fStruct)),
            ["float32"] = ((fTup, fStruct) => new FloatField(fTup, fStruct)),
            ["float64"] = ((fTup, fStruct) => new FloatField(fTup, fStruct)),
            ["int8"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["int16"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["int32"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["int64"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["uint8"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["uint16"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["uint32"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),
            ["uint64"] = ((fTup, fStruct) => new IntField(fTup, fStruct)),

            // Halo fields
            ["asciizptr"] = ((fTup, fStruct) => new StringField(fTup, fStruct)),
            ["structarray"] = ((fTup, fStruct) => new StructArrayField(fTup, fStruct)),
            ["tagreference"] = ((fTup, fStruct) => new TagReferenceField(fTup, fStruct)), // TODO
        };

        public static HaloField Build(PyObj fieldTuple, PyObj pyStruct)
        {
            // Unwrap the Tuple[str, Field]
            var name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();
            var pyField = fieldTuple.GetItem(PyObj.FromLong(1));
            var typeName = pyField["typestring"].ToString();

            ConstructFn constructor;
            if (!constructorDict.TryGetValue(typeName, out constructor))
            {
                throw new KeyNotFoundException(
                    string.Format("{0} is not yet supported by the GUI.", typeName));
            }

            return constructorDict[typeName](fieldTuple, pyStruct);
        }
    }

    public class EnumField : HaloField
    {
        public EnumField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct) { }
        public string Value
        {
            get { return "TODO: enum"; }
        }
    }

    public class FlagField : HaloField
    {
        public FlagField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct) { }
        public string Value
        {
            get { return "TODO: flag"; }
        }
    }

    public class FloatField : HaloField
    {
        public FloatField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct) { }
        public double Value
        {
            get { return pyStruct[Name].ToDouble(); }
            set { pyStruct[Name] = PyObj.FromDouble(value); }
        }
    }

    public class IntField : HaloField
    {
        public IntField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct) { }
        public long Value
        {
            get { return pyStruct[Name].ToLong(); }
            set { pyStruct[Name] = PyObj.FromLong(value); }
        }
    }

    public class StringField : HaloField
    {
        public StringField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct) { }
        public string Value
        {
            get { return pyStruct[Name].ToString(); }
            set { pyStruct[Name] = PyObj.FromString(value); }
        }
    }

    public class StructArrayField : HaloField
    {
        public StructArrayField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct)
        {
            Children = new List<PyObj>();
            var iter = pyStruct[Name].GetIter();
            var currStruct = iter.Next();
            while (currStruct != null)
            {
                Children.Add(currStruct);
                currStruct = iter.Next();
            }
        }

        public List<PyObj> Children { get; private set; }

        public string Value { get { return ""; } }
    }

    public class TagReferenceField : HaloField
    {
        public TagReferenceField(PyObj fieldTuple, PyObj pyStruct) : base(fieldTuple, pyStruct) { }

        public string TagClass
        {
            get { return pyStruct[Name]["first_class"].ToString(); }
        }

        public string Value
        {
            get { return pyStruct[Name].ToString(); }
        }
    }
}
