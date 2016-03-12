// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace PythonBinding {

    #pragma managed(push, off)
    /// Wraps a PyObject known to have a property_changed Event.
    class ObservablePyObject;

    /// Callback function to be passed into Python.
    int callback_thunk(ObservablePyObject* slf);
    #pragma managed(pop)

    public enum class FieldType {
        Ascii, Asciiz, RawData,
        Enum16, BitFlag, Float32, Float64,
        Int8, Int16, Int32, Int64,
        Uint8, UInt16, UInt32, UInt64,
        AsciizPtr, TagReference, StructArray,
        };

    typedef List<Tuple<String^, FieldType, Object^>^> FieldGroup;

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructProxy
    {
    public:
        HaloStructProxy(PyObject* halostruct);
        property FieldGroup^ Fields { FieldGroup^ get() { return _fields; } }
    private:
        ObservablePyObject* halostruct;
        FieldGroup^ _fields;
    };

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagProxy
    {
    public:
        HaloTagProxy(PyObject* halotag);
        property HaloStructProxy^ Header { HaloStructProxy^ get() { return _header; } }
        property HaloStructProxy^ Data { HaloStructProxy^ get() { return _data; } }

        HaloStructProxy^ HaloTagProxy::getData(); // Temporary
    private:
        PyObject* halotag;
        HaloStructProxy^ _header;
        HaloStructProxy^ _data;
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapProxy
    {
    public:
        HaloMapProxy(PyObject* map);
        property List<PythonBinding::HaloTagProxy^>^ Tags;
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
        property List<HaloMapProxy^>^ Maps { List<HaloMapProxy^>^ get() { return _maps; } }

    private:
        PythonInterpreter();
        PythonInterpreter(const PythonInterpreter%) { throw gcnew InvalidOperationException("PythonInterpreter cannot be copy-constructed"); }
        static PythonInterpreter m_instance;
        PyObject* halolib;
        property List<PythonBinding::HaloMapProxy^>^ _maps;
    };
}
