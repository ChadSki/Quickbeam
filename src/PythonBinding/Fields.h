#pragma once
#include "Stdafx.h"
using namespace System;

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
}