// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

#pragma once
#include "Stdafx.h"
#using <WindowsBase.dll>

using namespace System;
using namespace System::Collections::ObjectModel;

namespace PythonBinding {

#pragma managed(push, off)

    /// Wraps a PyObject known to have a property_changed Event.
    class ObservablePyObject;

    /// Callback function to be passed into Python.
    int callback_thunk(ObservablePyObject* slf);

#pragma managed(pop)

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructProxy
    {
    public:
        HaloStructProxy(PyObject* halostruct);
    private:
        PyObject* halostruct;
    };

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagProxy
    {
    public:
        HaloTagProxy(PyObject* halotag);
        HaloStructProxy^ getData();
    private:
        PyObject* halotag;
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapProxy
    {
    public:
        HaloMapProxy(PyObject* map);
        property ObservableCollection<PythonBinding::HaloTagProxy^>^ Tags;
        HaloTagProxy^ getGhost();
    private:
        PyObject* halomap;
    };

    public enum class HaloMemory { PC, CE };

    public ref class PythonInterpreter
    {
    public:
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %m_instance; } }
        void OpenMap(HaloMemory whichExe);
        void OpenMap(String^ filename);
        property ObservableCollection<PythonBinding::HaloMapProxy^>^ Maps;

    private:
        PythonInterpreter();
        PythonInterpreter(const PythonInterpreter%) { throw gcnew InvalidOperationException("PythonInterpreter cannot be copy-constructed"); }
        static PythonInterpreter m_instance;
        PyObject* halolib;
    };
}
