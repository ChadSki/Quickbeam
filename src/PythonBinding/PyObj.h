#include "Stdafx.h"

#pragma once
ref class PyObj
{
    PyObject* obj;
public:
    PyObj(PyObject* obj);

    /// <summary>
    /// If this Python object is callable, call it with the given arguments.
    /// </summary>
    PyObj^ CallObject(PyObj^ tupleArgs);

    /// <summary>
    /// Retrieve an attribute by name.
    /// </summary>
    PyObj^ GetAttrString(String^ attrName);

    /// <summary>
    /// If this Python object is iterable, get the iterator.
    /// </summary>
    PyObj^ GetIter();

    /// <summary>
    /// If this Python object supports [] indexing, get an item by its key.
    /// </summary>
    PyObj^ GetItem(PyObj^ key);

    /// <summary>
    /// Returns the string representation of this object.
    /// </summary>
    String^ Str();

    /// <summary>
    /// Print this object to the console for debugging purposes.
    /// </summary>
    void Print();
};

