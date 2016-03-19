#include "stdafx.h"
#include "Fields.h"

namespace PythonBinding
{
    String^ Field::Name::get() { return name; }

    FloatField::FloatField(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }

    double FloatField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto value = PyObject_CallObject(set_fn, nullptr);
        return PyFloat_AsDouble(value);
    }

    void FloatField::Value::set(double newvalue)
    {
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        PyObject_CallObject(set_fn, PyTuple_Pack(1,
            PyFloat_FromDouble(newvalue)));
    }

    IntField::IntField(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }

    long IntField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto value = PyObject_CallObject(set_fn, nullptr);
        return PyLong_AsLong(value);
    }

    void IntField::Value::set(long newvalue)
    {
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        PyObject_CallObject(set_fn, PyTuple_Pack(1,
            PyLong_FromLong(newvalue)));
    }

    UnknownField::UnknownField(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }
}