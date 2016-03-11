// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

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
        HaloStructProxy(PyObject* halostruct);
    private:
        PyObject* halostruct;
    };

    public ref class HaloTagProxy
    {
    public:
        HaloTagProxy(PyObject* halotag);
        HaloStructProxy^ getData();
    private:
        PyObject* halotag;
    };

    public ref class HaloMapProxy
    {
    public:
        HaloMapProxy();
        HaloTagProxy^ getGhost();
    private:
        PyObject* halomap;
    };
}
