// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

#pragma once
#include "Stdafx.h"
#include "HaloStruct.h"
#include "ObservablePyObject.h"

namespace PythonBinding {

    /// Common stuff for displaying in tree format
    public ref class ExplorerNode abstract
    {
    public:
        virtual property ObservableCollection<ExplorerNode^>^ Children;
        virtual property String^ Name;
        virtual property String^ Suffix;

    };

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagNode : ExplorerNode
    {
        PyObject* halotag;
        HaloStructViewModel^ header;
        HaloStructViewModel^ data;
        ObservableCollection<ExplorerNode^>^ noChildren{};  // Should remain empty

    public:
        HaloTagNode(PyObject* halotag);
        property HaloStructViewModel^ Header { HaloStructViewModel^ get() { return header; } }
        property HaloStructViewModel^ Data { HaloStructViewModel^ get() { return data; } }
        property String^ FirstClass
        {
            String^ get() { return (String^)(header->Get("first_class")); }
        }
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() override { return noChildren; }
        }
        virtual property String^ Name
        {
            String^ get() override { return (String^)(header->Get("name")); }
        }
        virtual property String^ Suffix { String^ get() override { return "tag"; } }
        virtual String^ ToString() override;
    };

    public ref class HaloTagClassNode : ExplorerNode
    {
        String^ className;
        ObservableCollection<ExplorerNode^>^ tags;
    public:
        HaloTagClassNode(String^ className, ObservableCollection<ExplorerNode^>^ tags)
        {
            this->className = className;
            this->tags = tags;
        }
        virtual property String^ Name { String^ get() override { return className; } }
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() override { return tags; }
        }
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapNode : ExplorerNode
    {
        PyObject* halomap;
        ObservableCollection<ExplorerNode^>^ tagClasses{};

    public:
        HaloMapNode(PyObject* map);
        HaloTagNode^ getGhost();
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() override { return tagClasses; }
        }
        virtual property String^ Name { String^ get() override { return ToString(); } }
        virtual property String^ Suffix { String^ get() override { return "map"; } }
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
        property ObservableCollection<ExplorerNode^>^ maps;

        bool initialized = false;

    public:
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %instance; } }
        void OpenMap(HaloMemory whichExe);
        void OpenMap(String^ filename);
        virtual property ObservableCollection<ExplorerNode^>^ Children
        {
            ObservableCollection<ExplorerNode^>^ get() { return maps; }
        }
        virtual String^ ToString() override;
    };
}
