using ICSharpCode.TreeView;
using NimbusSharp;
using System;
using System.Linq;
using System.Collections.Generic;

// Constructor type signature
using HaloFieldNodeConstructFn = System.Func<
        NimbusSharp.HaloField,
        NimbusSharpGUI.TagEditor.HaloFieldThunk>;
using PythonBinding;
using System.ComponentModel;

namespace NimbusSharpGUI.TagEditor
{
    public class HaloFieldNode : SharpTreeNode
    {
        protected HaloFieldNode(HaloFieldThunk hfThunk, HaloStruct hStruct)
        {
            FieldThunk = hfThunk;

            // Load children eagerly
            var sa = hfThunk as StructArrayFieldNode;
            if (sa != null)
            {

                for (int i = 0; i < sa.Children.Count; i++)
                {
                    Children.Add(
                        new HaloStructNode(
                            new HaloStruct(sa.Children[i], hStruct.ParentTag),
                            string.Format("[{0}]", i)));
                }

                // Auto-expand if there aren't multiple children
                if (sa.Children.Count < 2) { IsExpanded = true; }
            }
        }

        public HaloFieldThunk FieldThunk { get; private set; }

        public override object Text { get { return FieldThunk.Name; } }

        private static readonly Dictionary<string, HaloFieldNodeConstructFn> constructorDict = new Dictionary<string, HaloFieldNodeConstructFn>
        {
            // basic fields
            ["ascii"] =   (hf => new StringFieldNode(hf as StringField)),
            ["asciiz"] =  (hf => new StringFieldNode(hf as StringField)),
            ["rawdata"] = (hf => new StringFieldNode(hf as StringField)),
            ["enum16"] =  (hf => new EnumFieldNode(hf as EnumField)),
            ["flag"] =    (hf => new FlagFieldNode(hf as FlagField)),
            ["float32"] = (hf => new FloatFieldNode(hf as FloatField)),
            ["float64"] = (hf => new FloatFieldNode(hf as FloatField)),
            ["int8"] =    (hf => new IntFieldNode(hf as IntField)),
            ["int16"] =   (hf => new IntFieldNode(hf as IntField)),
            ["int32"] =   (hf => new IntFieldNode(hf as IntField)),
            ["int64"] =   (hf => new IntFieldNode(hf as IntField)),
            ["uint8"] =   (hf => new IntFieldNode(hf as IntField)),
            ["uint16"] =  (hf => new IntFieldNode(hf as IntField)),
            ["uint32"] =  (hf => new IntFieldNode(hf as IntField)),
            ["uint64"] =  (hf => new IntFieldNode(hf as IntField)),

            // Halo fields
            ["asciizptr"] =    (hf => new StringFieldNode(hf as StringField)),
            ["structarray"] =  (hf => new StructArrayFieldNode(hf as StructArrayField)),
            ["tagreference"] = (hf => new TagReferenceFieldNode(hf as TagReferenceField)),
        };

        public static HaloFieldNode Build(HaloStruct hStruct, HaloField hField)
        {
            var typeName = hField.TypeName;
            HaloFieldNodeConstructFn constructor;
            if (!constructorDict.TryGetValue(typeName, out constructor))
            {
                throw new KeyNotFoundException(
                    string.Format("{0} is not yet supported by the GUI.", typeName));
            }

            return new HaloFieldNode(constructorDict[typeName](hField), hStruct);
        }
    }

    public interface HaloFieldThunk
    {
        string Name { get; }
    }

    public class EnumFieldNode : HaloFieldThunk
    {
        public EnumFieldNode(EnumField hField) { Field = hField; }
        protected EnumField Field { get; set; }
        public string Name { get { return Field.Name; } }
        public string Value
        {
            get { return Field.Value; }
            set { throw new NotImplementedException(); }
        }
    }

    public class FlagFieldNode : HaloFieldThunk
    {
        public FlagFieldNode(FlagField hField) { Field = hField; }
        protected FlagField Field { get; set; }
        public string Name { get { return Field.Name; } }
        public string Value
        {
            get { return Field.Value; }
            set { throw new NotImplementedException(); }
        }
    }

    public class FloatFieldNode : HaloFieldThunk
    {
        public FloatFieldNode(FloatField hField) { Field = hField; }
        protected FloatField Field { get; set; }
        public string Name { get { return Field.Name; } }
        public double Value
        {
            get { return Field.Value; }
            set { Field.Value = value; }
        }
    }

    public class IntFieldNode : HaloFieldThunk
    {
        public IntFieldNode(IntField hField) { Field = hField; }
        protected IntField Field { get; set; }
        public string Name { get { return Field.Name; } }
        public long Value
        {
            get { return Field.Value; }
            set { Field.Value = value; }
        }
    }

    public class StringFieldNode : HaloFieldThunk
    {
        public StringFieldNode(StringField hField) { Field = hField; }
        protected StringField Field { get; set; }
        public string Name { get { return Field.Name; } }
        public string Value
        {
            get { return Field.Value; }
            set { Field.Value = value; }
        }
    }

    public class StructArrayFieldNode : HaloFieldThunk
    {
        public StructArrayFieldNode(StructArrayField hField) { Field = hField; }
        protected StructArrayField Field { get; set; }
        public string Name { get { return Field.Name; } }
        public List<PyObj> Children
        {
            get { return Field.Children; }
        }
    }

    public class TagReferenceFieldNode : HaloFieldThunk, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TagReferenceFieldNode(TagReferenceField hField)
        {
            Field = hField;
        }

        protected TagReferenceField Field { get; set; }

        public string Name { get { return Field.Name; } }

        public IEnumerable<string> PossibleTagClasses
        {
            get { return Field.PossibleTagClasses; }
        }

        public IEnumerable<string> PossibleTags
        {
            get { return Field.PossibleTags; }
        }

        public string SelectedTagClass
        {
            get { return Field.SelectedTagClass; }
            set
            {
                Field.SelectedTagClass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PossibleTags"));
            }
        }

        public string SelectedTag
        {
            get { return Field.SelectedTag; }
            set { Field.SelectedTag = value; }
        }
    }
}
