#include "Stdafx.h"

#pragma once
namespace PythonBinding
{
    ref class PyIter;

    public ref class PyObj
    {
    protected:
        ~PyObj();
    public:
        PyObject* obj;
        PyObj(PyObject* obj);

        /// <summary>
        /// Returns the double representation of this object.
        /// </summary>
        double AsDouble();

        /// <summary>
        /// Returns the integer representation of this object.
        /// </summary>
        long AsLong();

        /// <summary>
        /// Returns the string representation of this object.
        /// </summary>
        String^ AsStr();

        /// <summary>
        /// Get a method by name and call it.
        /// </summary>
        PyObj^ CallMethod(String^ methodName, PyObj^ tupleArgs);

        /// <summary>
        /// Call this Python object with the given arguments.
        /// </summary>
        PyObj^ CallObject(PyObj^ tupleArgs);

        /// <summary>
        /// Retrieve an attribute by name.
        /// </summary>
        PyObj^ GetAttrString(String^ attrName);

        /// <summary>
        /// If this Python object is iterable, get the iterator.
        /// </summary>
        PyIter^ GetIter();

        /// <summary>
        /// If this Python object supports [] indexing, get an item by its key.
        /// </summary>
        PyObj^ GetItem(PyObj^ key);

        PyObj^ Module_GetAttr(String^ memberName);

        /// <summary>
        /// Print this object to the console for debugging purposes.
        /// </summary>
        void Print();

        static PyObj^ FromDouble(double value);

        static PyObj^ FromLong(long value);

        static PyObj^ FromStr(String^ value);

        // TODO make the variadic version
        static PyObj^ PackTuple1(PyObj^ value);
        static PyObj^ PackTuple2(PyObj^ value0, PyObj^ value1);
    };

    public ref class PyIter : PyObj
    {
    public:
        PyIter(PyObject* obj) : PyObj(obj) {}
        PyObj^ Next();
    };
}
