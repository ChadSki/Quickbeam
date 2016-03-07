// PythonBinding.h

#pragma once
#include "Stdafx.h"

using namespace System;

namespace PythonBinding {

#pragma managed(push, off)

    /// Wraps a PyObject known to have a property_changed Event.
    public class PyObject_Thunk
    {
    public:
        PyObject_Thunk(PyObject* po);
        PyObject_Thunk(const PyObject_Thunk &po);
        ~PyObject_Thunk();

        /// Triggered whenever a property of the bound PyObject is updated.
        void OnPropertyChanged();
        PyObject* _po;
    };

    /// Callback function to be passed into Python.
    int callback(PyObject_Thunk* slf);

#pragma managed(pop)

    public ref class HaloStructProxy
    {
    public:
        HaloStructProxy();
        HaloStructProxy(const HaloStructProxy^ & Name);
        ~HaloStructProxy();
    };

    public ref class HaloTagProxy
    {
    public:
        HaloTagProxy(PyObject* tag);
        HaloTagProxy(const HaloTagProxy^ & Name);
        ~HaloTagProxy();
    private:
        PyObject* tag;
    };

    public ref class HaloMapProxy
    {
    public:
        HaloMapProxy();
        HaloMapProxy(const HaloMapProxy^ & other);
        ~HaloMapProxy();

        /// Pseudo-main function for developing purposes.
        HaloTagProxy^ getGhost();

    private:
        PyObject_Thunk* halomap;
    };
}
