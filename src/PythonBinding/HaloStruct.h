#pragma once
#include "Stdafx.h"
#include "ObservablePyObject.h"

namespace PythonBinding
{
    /// TODO
    public ref class Field abstract
    {
    protected:
        PyObject* field;
        String^ name;
    public:
        property String^ Name { String^ get(); }
    };

    /// TODO
    public ref class FloatField : Field
    {
    public:
        FloatField(String^ name, PyObject* field);
        property double Value { double get(); void set(double newvalue); }
    };

    /// TODO
    public ref class IntField : Field
    {
    public:
        IntField(String^ name, PyObject* field);
        property long Value { long get(); void set(long newvalue); }
    };

    /// TODO
    public ref class UnknownField : Field
    {
    public:
        UnknownField(String^ name, PyObject* field);
        property String^ Repr { String^ get() { return "TODO"; } }
    };

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructViewModel
    {
        ObservablePyObject* halostruct;
        ObservableCollection<Field^>^ fields;

    public:
        HaloStructViewModel(PyObject* halostruct);
        property ObservableCollection<Field^>^ Fields {
            ObservableCollection<Field^>^ get() { return fields; }
        }
        Object^ Get(String^ attrName);
        virtual String^ ToString() override;
    };
}