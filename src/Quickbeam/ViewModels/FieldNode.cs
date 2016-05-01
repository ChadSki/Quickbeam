using ICSharpCode.TreeView;
using PythonBinding;

namespace Quickbeam.ViewModels
{
    public abstract class FieldNode : SharpTreeNode
    {
        public string Name { get; protected set; }

        public PyObj FieldObject { get; protected set; }

        public FieldNode(string name, PyObj field)
        {
            Name = name;
            FieldObject = field;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class BytesField : FieldNode
    {
        public BytesField(string name, PyObj field) : base(name, field) { }
        public string Value {
            get { return FieldObject.CallMethod("getf", null).AsStr(); }
        }
    }

    public class FloatField : FieldNode
    {
        public FloatField(string name, PyObj field) : base(name, field) { }
        public double Value
        {
            get { return FieldObject.CallMethod("getf", null).AsDouble(); }
        }
    }

    public class IntField : FieldNode
    {
        public IntField(string name, PyObj field) : base(name, field) { }
        public long Value
        {
            get { return FieldObject.CallMethod("getf", null).AsLong(); }
        }
    }

    public class StringField : FieldNode
    {
        public StringField(string name, PyObj field) : base(name, field) { }
        public string Value
        {
            get { return FieldObject.CallMethod("getf", null).AsStr(); }
        }
    }

    public class UnknownField : FieldNode
    {
        public UnknownField(string name, PyObj field) : base(name, field) { }
        public string Value
        {
            get { return FieldObject.CallMethod("getf", null).AsStr(); }
        }
    }
}
