using PythonBinding;
using System;
using System.Linq;

// Constructor type signature
using HaloFieldConstructFn = System.Func<
        PythonBinding.PyObj,
        PythonBinding.PyObj,
        NimbusSharp.HaloStruct,
        NimbusSharp.HaloField>;

using System.Collections.Generic;

namespace NimbusSharp
{
    public class HaloField
    {
        private PyObj pyStruct;
        protected PyObj pyField;

        /// Access this field by name and get its value as a PyObj.
        protected PyObj PyValue
        {
            get { return pyStruct[Name]; }
            set { pyStruct[Name] = value; }
        }

        public HaloStruct ParentStruct { get; private set; }

        public string Name { get; private set; }

        public string TypeName
        {
            get { return pyField["typestring"].ToString(); }
        }

        public long Offset
        {
            get { return pyField["offset"].ToLong(); }
        }

        protected HaloField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct)
        {
            // Unwrap the Tuple[str, Field]
            Name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();
            pyField = fieldTuple.GetItem(PyObj.FromLong(1));
            this.pyStruct = pyStruct;
            ParentStruct = hStruct;
        }

        private static readonly Dictionary<string, HaloFieldConstructFn> constructorDict = new Dictionary<string, HaloFieldConstructFn>
        {
            // basic fields
            ["ascii"] =   ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["asciiz"] =  ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["rawdata"] = ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["enum16"] =  ((fTup, pStruct, hStruct) => new EnumField(fTup, pStruct, hStruct)),
            ["flag"] =    ((fTup, pStruct, hStruct) => new FlagField(fTup, pStruct, hStruct)),
            ["float32"] = ((fTup, pStruct, hStruct) => new FloatField(fTup, pStruct, hStruct)),
            ["float64"] = ((fTup, pStruct, hStruct) => new FloatField(fTup, pStruct, hStruct)),
            ["int8"] =    ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["int16"] =   ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["int32"] =   ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["int64"] =   ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint8"] =   ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint16"] =  ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint32"] =  ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),
            ["uint64"] =  ((fTup, pStruct, hStruct) => new IntField(fTup, pStruct, hStruct)),

            // Halo fields
            ["asciizptr"] =    ((fTup, pStruct, hStruct) => new StringField(fTup, pStruct, hStruct)),
            ["structarray"] =  ((fTup, pStruct, hStruct) => new StructArrayField(fTup, pStruct, hStruct)),
            ["tagreference"] = ((fTup, pStruct, hStruct) => new TagReferenceField(fTup, pStruct, hStruct)),
        };

        public static HaloField Build(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct)
        {
            // Unwrap the Tuple[str, Field]
            var name = fieldTuple.GetItem(PyObj.FromLong(0)).ToString();
            var pyField = fieldTuple.GetItem(PyObj.FromLong(1));
            var typeName = pyField["typestring"].ToString();

            HaloFieldConstructFn constructor;
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
            get { return PyValue.ToDouble(); }
            set { PyValue = PyObj.FromDouble(value); }
        }
    }

    public class IntField : HaloField
    {
        public IntField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public long Value
        {
            get { return PyValue.ToLong(); }
            set { PyValue = PyObj.FromLong(value); }
        }
    }

    public class StringField : HaloField
    {
        public StringField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct) { }
        public string Value
        {
            get { return PyValue.ToString(); }
            set { PyValue = PyObj.FromString(value); }
        }
    }

    public class StructArrayField : HaloField
    {
        public StructArrayField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct)
        {
            Children = new List<PyObj>();
            var iter = PyValue.GetIter();
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
        public IEnumerable<string> PossibleTagClasses { get; private set; }

        // `name (ident)` format so they're unique?
        public IEnumerable<string> PossibleTags { get; private set; }

        private HaloMap ParentMap { get { return ParentStruct.ParentTag.ParentMap; } }

        public TagReferenceField(PyObj fieldTuple, PyObj pyStruct, HaloStruct hStruct) : base(fieldTuple, pyStruct, hStruct)
        {
            PossibleTagClasses = ParentMap.TagClasses;
            PossibleTags = ParentMap.TagsOfClass(SelectedTagClass).Select(x => x.UniqueName);
        }

        string _selectedClass = null;
        public string SelectedTagClass
        {
            get
            {
                if (_selectedClass == null)
                {
                    var selectedTag = PyValue;
                    return selectedTag.IsNone() ? "" : selectedTag["first_class"].ToString();
                }
                return _selectedClass;
            }
            set
            {
                _selectedClass = value;
                PossibleTags = ParentMap.TagsOfClass(_selectedClass).Select(x => x.UniqueName);
            }
        }

        public string SelectedTag
        {
            get { return ParentMap.TagFromPyObj(PyValue).UniqueName; }
            set
            {
                var x = ParentMap.TagFromUniqueName(value);
                var y = x.pyTag;
                Console.WriteLine("====");
                Console.WriteLine(y.ToString());
                PyValue = y;
                Console.WriteLine(PyValue.ToString());
            }
        }
    }
}
