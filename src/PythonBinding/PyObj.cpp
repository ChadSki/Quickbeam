#include "stdafx.h"
#include "PyObj.h"

PyObj::PyObj(PyObject * obj)
{
    this->obj = obj;
    Py_INCREF(this->obj);
}

PyObj::~PyObj()
{
    Py_DECREF(this->obj);
    this->obj = nullptr;
}

PyObj^ PyObj::CallObject(PyObj^ tupleArgs)
{
    auto result = PyObject_CallObject(this->obj, tupleArgs->obj);
    if (result == nullptr) {
        throw gcnew NullReferenceException("Calling object failed.");
    }
    return gcnew PyObj(result);
}

PyObj^ PyObj::GetAttrString(String^ attrName)
{
    auto rawName = Marshal::StringToHGlobalAnsi(attrName);
    auto result = PyObject_GetAttrString(this->obj, (char*)(void*)rawName);
    Marshal::FreeHGlobal(rawName);
    if (result == nullptr) {
        throw gcnew NullReferenceException(
            String::Format("Could not find attribute `{}`", attrName));
    }
    return gcnew PyObj(result);
}

PyObj^ PyObj::GetIter()
{
    auto result = PyObject_GetIter(this->obj);
    if (result == nullptr) {
        throw gcnew NullReferenceException(
            "Failed to iterate over object.");
    }
    return gcnew PyObj(result);
}

PyObj^ PyObj::GetItem(PyObj^ key)
{
    auto result = PyObject_GetItem(this->obj, key->obj);
    if (result == nullptr) {
        throw gcnew NullReferenceException("Failed to get item from key.");
    }
    return gcnew PyObj(result);
}

String^ PyObj::Str()
{
    auto utf8bytes = PyUnicode_AsUTF8AndSize(PyObject_Str(this->obj), nullptr);
    return gcnew String(utf8bytes);
}

void PyObj::Print()
{
    PyObject_Print(this->obj, stdout, Py_PRINT_RAW);
    std::cout << std::endl;
}
