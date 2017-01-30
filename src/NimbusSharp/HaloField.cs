using PythonBinding;
using System;

// Constructor type signature
using ConstructFn = System.Func<
        PythonBinding.PyObj,
        PythonBinding.PyObj,
        NimbusSharp.HaloStruct,
        NimbusSharp.HaloField>;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NimbusSharp
{
    public class HaloField
    {
        protected PyObj pyStruct;
        protected PyObj pyField;

        public HaloStruct ParentStruct { get; private set; }

        protected HaloField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct)
        {
            // Unwrap the Tuple[str, Field]
            Name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();
            pyField = fieldTuple.GetItem(PyObj.FromLong(1));
            this.pyStruct = pyStruct;
            ParentStruct = hStruct;
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
            ["ascii"] = ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["asciiz"] = ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["rawdata"] = ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["enum16"] = ((fTup, pStruct, hStruct) => new EnumField(fTup, pStruct, hStruct)),
            ["flag"] = ((fTup, pStruct, hStruct) => new FlagField(fTup, pStruct, hStruct)),
            ["float32"] = ((fTup, pStruct, hStruct) => new FloatField(fTup, pStruct, hStruct)),
            ["float64"] = ((fTup, pStruct, hStruct) => new FloatField(fTup, pStruct, hStruct)),
            ["int8"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["int16"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["int32"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["int64"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint8"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint16"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint32"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint64"] = ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),

            // Halo fields
            ["asciizptr"] = ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["structarray"] = ((fTup, pStruct, hStruct) => new StructArrayField(fTup, pStruct, hStruct)),
            ["tagreference"] = ((fTup, pStruct, hStruct) => new TagReferenceField(fTup, pStruct, hStruct)), // TODO
        };

        public static HaloField Build(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct)
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

            return constructorDict[typeName](fieldTuple, pyStruct, hStruct);
        }
    }

    public class EnumField : HaloField
    {
        public EnumField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public string Value
        {
            get { return "TODO: enum"; }
        }
    }

    public class FlagField : HaloField
    {
        public FlagField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public string Value
        {
            get { return "TODO: flag"; }
        }
    }

    public class FloatField : HaloField
    {
        public FloatField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public double Value
        {
            get { return pyStruct[Name].ToDouble(); }
            set { pyStruct[Name] = PyObj.FromDouble(value); }
        }
    }

    public class IntField : HaloField
    {
        public IntField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public long Value
        {
            get { return pyStruct[Name].ToLong(); }
            set { pyStruct[Name] = PyObj.FromLong(value); }
        }
    }

    public class StringField : HaloField
    {
        public StringField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public string Value
        {
            get { return pyStruct[Name].ToString(); }
            set { pyStruct[Name] = PyObj.FromString(value); }
        }
    }

    public class StructArrayField : HaloField
    {
        public StructArrayField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct)
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
    }

    public class TagReferenceField : HaloField
    {
        // Somehow needs to refer to the map object.
        public List<string> PossibleTagClasses { get; private set; }

        // `name (ident)` format so they're unique?
        public ObservableCollection<string> PossibleTags { get; private set; } = new ObservableCollection<string>();

        public TagReferenceField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct)
        {
            // TODO: Load these from the map
            PossibleTagClasses = ParentStruct.ParentTag.ParentMap.TagClasses;
        }

        public string SelectedTagClass
        {
            get
            {
                var x = pyStruct[Name];
                var y = x["first_class"];
                var z = y.ToString();
                return z;
            }
            set { throw new NotImplementedException(); }
        }

        public string SelectedTag
        {
            get { return pyStruct[Name].ToString(); }
            set { throw new NotImplementedException(); }
        }
    }
}
