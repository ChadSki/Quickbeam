using PythonBinding;

namespace Quickbeam.ViewModels
{
    public abstract class Field
    {
        public string Name { get; protected set; }

        protected PyObj FieldObject;

        protected Field(string name, PyObj field)
        {
            Name = name;
            FieldObject = field;
        }
    }

    public class BytesField : Field
    {
        public BytesField(string name, PyObj field) : base(name, field) { }
        public string Value {
            get { return FieldObject.CallMethod("getf", null).Str(); }
        }
    }

    public class FloatField : Field
    {
        public FloatField(string name, PyObj field) : base(name, field) { }
        public double Value
        {
            get { return FieldObject.CallMethod("getf", null).AsDouble(); }
        }
    }

    public class IntField : Field
    {
        public IntField(string name, PyObj field) : base(name, field) { }
        public long Value
        {
            get { return FieldObject.CallMethod("getf", null).AsLong(); }
        }
    }

    public class StringField : Field
    {
        public StringField(string name, PyObj field) : base(name, field) { }
        public string Value
        {
            get { return FieldObject.CallMethod("getf", null).Str(); }
        }
    }

    public class UnknownField : Field
    {
        public UnknownField(string name, PyObj field) : base(name, field) { }
        public string Value
        {
            get { return FieldObject.CallMethod("getf", null).Str(); }
        }
    }
}
