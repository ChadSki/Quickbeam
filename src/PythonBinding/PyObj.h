#include "Stdafx.h"

#pragma once
namespace PythonBinding
{
    ref class PyIter;

    public ref class PyObj
    {
    protected:
        PyObject* obj;
        ~PyObj();
    public:
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

        /// <summary>
        /// Print this object to the console for debugging purposes.
        /// </summary>
        void Print();
    };

    public ref class PyIter : PyObj
    {
    public:
        PyIter(PyObject* obj) : PyObj(obj) {}
        PyObj^ Next();
    };
}
