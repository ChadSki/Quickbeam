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

    public ref class FieldProxy
    {
    public:
        FieldProxy(PyObject* field);
        property Object^ Value;
    };

    typedef Tuple<FieldType, FieldProxy^> FieldEntry;

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructProxy
    {
        ObservablePyObject* halostruct;
        Dictionary<String^, FieldEntry^>^ fields;

    public:
        HaloStructProxy(PyObject* halostruct);
        property Dictionary<String^, FieldEntry^>^ Fields {
            Dictionary<String^, FieldEntry^>^ get() { return fields; }
        }
        virtual String^ ToString() override;
    };

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagProxy
    {
        PyObject* halotag;
        HaloStructProxy^ header;
        HaloStructProxy^ data;

    public:
        HaloTagProxy(PyObject* halotag);
        property HaloStructProxy^ Header { HaloStructProxy^ get() { return header; } }
        property HaloStructProxy^ Data { HaloStructProxy^ get() { return data; } }
        virtual String^ ToString() override;
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapProxy
    {
        PyObject* halomap;
        List<HaloTagProxy^>^ tags;

    public:
        HaloMapProxy(PyObject* map);
        property List<HaloTagProxy^>^ Tags { List<HaloTagProxy^>^ get() { return tags; } }
        HaloTagProxy^ getGhost();
        virtual String^ ToString() override;
    };

    public enum class HaloMemory { PC, CE };

    /// Top-level, allows you to open Halo maps
    public ref class PythonInterpreter
    {
        PythonInterpreter();
        PythonInterpreter(const PythonInterpreter%) {
            throw gcnew InvalidOperationException(
                "PythonInterpreter cannot be copy-constructed");
        }
        static PythonInterpreter instance;
        PyObject* halolib;
        property List<PythonBinding::HaloMapProxy^>^ maps;

    public:
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %instance; } }
        void OpenMap(HaloMemory whichExe);
        void OpenMap(String^ filename);
        property List<HaloMapProxy^>^ Maps { List<HaloMapProxy^>^ get() { return maps; } }
        virtual String^ ToString() override;
    };
}
