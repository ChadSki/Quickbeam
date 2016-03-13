// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

#pragma once
#include "Stdafx.h"
#using <WindowsBase.dll>
#using <System.Core.dll>

using namespace System;
using namespace System::Linq;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;

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

    /// Common stuff for displaying in tree format
    public ref class ExplorerNode abstract
    {
    public:
        virtual property ObservableCollection<ExplorerNode^>^ Children;
        virtual property String^ Name;
        virtual property String^ Suffix;
        property Boolean^ IsFolder { Boolean^ get() { return Children->Count > 0; } }

    };

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagProxy : ExplorerNode
    {
        PyObject* halotag;
        HaloStructProxy^ header;
        HaloStructProxy^ data;
        ObservableCollection<ExplorerNode^>^ noChildren{};  // Should remain empty

    public:
        HaloTagProxy(PyObject* halotag);
        property HaloStructProxy^ Header { HaloStructProxy^ get() { return header; } }
        property HaloStructProxy^ Data { HaloStructProxy^ get() { return data; } }
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() override { return noChildren; }
        }
        virtual property String^ Name { String^ get() override { return ToString(); } }
        virtual property String^ Suffix { String^ get() override { return "tag"; } }
        virtual String^ ToString() override;
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapProxy : ExplorerNode
    {
        PyObject* halomap;
        ObservableCollection<ExplorerNode^>^ tags;

    public:
        HaloMapProxy(PyObject* map);
        HaloTagProxy^ getGhost();
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() override { return tags; }
        }
        virtual property String^ Name { String^ get() override { return ToString(); } }
        virtual property String^ Suffix { String^ get() override { return "map"; } }
        virtual String^ ToString() override;
    };

    public enum class HaloMemory { PC, CE };

    /// Top-level, allows you to open Halo maps
    public ref class PythonInterpreter : ExplorerNode
    {
        PythonInterpreter();
        PythonInterpreter(const PythonInterpreter%) {
            throw gcnew InvalidOperationException(
                "PythonInterpreter cannot be copy-constructed");
        }
        static PythonInterpreter instance;
        PyObject* halolib;
        property ObservableCollection<ExplorerNode^>^ maps;

    public:
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %instance; } }
        void OpenMap(HaloMemory whichExe);
        void OpenMap(String^ filename);
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() override { return maps; }
        }
        virtual property String^ Name { String^ get() override { return "Quickbeam"; } }
        virtual property String^ Suffix { String^ get() override { return "Python Environment"; } }
        virtual String^ ToString() override;
    };
}
