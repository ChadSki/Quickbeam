// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

#pragma once
#include "Stdafx.h"
#include "PyObj.h"
#include "HaloStruct.h"
#include "ObservablePyObject.h"

namespace CrappyCppBinding {

    /// Common stuff for displaying in tree format
    public ref class ExplorerNode abstract
    {
    public:
        virtual property ObservableCollection<ExplorerNode^>^ Children;
        virtual property String^ Name;
        virtual property String^ Suffix;
    };

    // For brevity
    typedef ObservableCollection<ExplorerNode^> ChildNodes;

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagNode : ExplorerNode
    {
        PyObject* halotag;
        HaloStructViewModel^ header;
        HaloStructViewModel^ data;
        ChildNodes^ noChildren{};  // Should remain empty

    public:
        HaloTagNode(PyObject* halotag);
        property HaloStructViewModel^ Header { HaloStructViewModel^ get() { return header; } }
        property HaloStructViewModel^ Data { HaloStructViewModel^ get() { return data; } }
        property String^ FirstClass { String^ get(); }
        virtual property String^ Name { String^ get() override; }
        virtual property String^ Suffix { String^ get() override; }
        virtual property ChildNodes^ Children { ChildNodes^ get() override; }
        virtual String^ ToString() override;
    };

    public ref class HaloTagClassNode : ExplorerNode
    {
        String^ className;
        ChildNodes^ tags;

    public:
        HaloTagClassNode(String^ className, ChildNodes^ tags);
        virtual property String^ Name { String^ get() override; }
        virtual property ChildNodes^ Children { ChildNodes^ get() override; }
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapNode : ExplorerNode
    {
        PyObject* halomap;
        ChildNodes^ tagClasses{};

    public:
        HaloMapNode(PyObject* map);
        HaloTagNode^ getArbitraryTag();
        virtual property String^ Name { String^ get() override { return ToString(); } }
        virtual property String^ Suffix { String^ get() override { return "map"; } }
        virtual property ChildNodes^ Children { ChildNodes^ get() override; }
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
        PyObject* halomap_class;
        property ChildNodes^ maps;

        bool initialized = false;

    public:
        static PyObj^ Initialize();
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %instance; } }
        void OpenMap(HaloMemory whichExe);
        void OpenMap(String^ filename);
        property ChildNodes^ Children { ChildNodes^ get(); }
        String^ ToString() override;
    };
}
