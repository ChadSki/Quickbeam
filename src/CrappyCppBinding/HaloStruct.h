#pragma once
#include "Stdafx.h"
#include "PyObj.h"
#include "ObservablePyObject.h"

namespace CrappyCppBinding
{
    /// Base class for Field objects in a struct
    public ref class Field abstract
    {
    protected:
        PyObject* field;
        String^ name;
    public:
        Field(String^ name, PyObject* field);
        property String^ Name { String^ get(); }
    };

    public ref class BytesField : Field
    {
    public:
        BytesField(String^ name, PyObject* field) : Field(name, field) {};
        property String^ Value { String^ get(); void set(String^ newvalue); }
    };

    public ref class FloatField : Field
    {
    public:
        FloatField(String^ name, PyObject* field) : Field(name, field) {};
        property double Value { double get(); void set(double newvalue); }
    };

    public ref class IntField : Field
    {
    public:
        IntField(String^ name, PyObject* field) : Field(name, field) {};
        property long Value { long get(); void set(long newvalue); }
    };

    public ref class StringField : Field
    {
    public:
        StringField(String^ name, PyObject* field) : Field(name, field) {};
        property String^ Value { String^ get(); void set(String^ newvalue); }
    };

    public ref class UnknownField : Field
    {
    public:
        UnknownField(String^ name, PyObject* field) : Field(name, field) {}
        property String^ Value { String^ get(); }
    };

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructViewModel
    {
        ObservablePyObject* halostruct;
        ObservableCollection<Field^>^ fields;

    public:
        HaloStructViewModel(PyObject* halostruct);
        HaloStructViewModel(PyObj^ halostruct);
        property ObservableCollection<Field^>^ Fields {
            ObservableCollection<Field^>^ get() { return fields; }
        }
        Object^ Get(String^ attrName);
        virtual String^ ToString() override;
    };
}