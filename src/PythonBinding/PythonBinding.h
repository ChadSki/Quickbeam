// TODO - I haven't written copy constructors or
// destructors = memory leak? ¯\_(ツ)_/¯

#pragma once
#include "Stdafx.h"
#using <WindowsBase.dll>
#using <System.Core.dll>

using namespace System;
using namespace System::Linq;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;

namespace PythonBinding {

    #pragma managed(push, off)
    /// Wraps a PyObject known to have a property_changed Event.
    class ObservablePyObject;

    /// Callback function to be passed into Python.
    int callback_thunk(ObservablePyObject* slf);
    #pragma managed(pop)

    public ref class Field abstract
    {
    protected:
        PyObject* field;
        String^ name;
    public:
        property String^ Name { String^ get() { return name; } }
    };

    public ref class UnknownField : Field
    {
    public:
        UnknownField(String^ name, PyObject* field)
        {
            this->name = name;
            this->field = field;
        }
        property String^ Repr { String^ get() { return "TODO"; } }
    };

    public ref class FloatField : Field
    {
    public:
        FloatField(String^ name, PyObject* field)
        {
            this->name = name;
            this->field = field;
        }
        property double Value
        {
            double get()
            {
                auto set_fn = PyObject_GetAttrString(this->field, "getf");
                auto value = PyObject_CallObject(set_fn, nullptr);
                return PyFloat_AsDouble(value);
            }
            void set(double newvalue)
            {
                auto set_fn = PyObject_GetAttrString(this->field, "setf");
                PyObject_CallObject(set_fn, PyTuple_Pack(1,
                    PyFloat_FromDouble(newvalue)));
            }
        }
    };

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructViewModel
    {
        ObservablePyObject* halostruct;
        List<Field^>^ fields;

    public:
        HaloStructViewModel(PyObject* halostruct);
        property List<Field^>^ Fields {
            List<Field^>^ get() { return fields; }
        }
        Object^ Get(String^ attrName);
        virtual String^ ToString() override;
    };

    /// Common stuff for displaying in tree format
    public ref class ExplorerNode abstract
    {
    public:
        virtual property List<ExplorerNode^>^ Children;
        virtual property String^ Name;
        virtual property String^ Suffix;

    };

    /// Wraps a PyObject known to be a HaloTag
    public ref class HaloTagNode : ExplorerNode
    {
        PyObject* halotag;
        HaloStructViewModel^ header;
        HaloStructViewModel^ data;
        List<ExplorerNode^>^ noChildren{};  // Should remain empty

    public:
        HaloTagNode(PyObject* halotag);
        property HaloStructViewModel^ Header { HaloStructViewModel^ get() { return header; } }
        property HaloStructViewModel^ Data { HaloStructViewModel^ get() { return data; } }
        property String^ FirstClass
        {
            String^ get() { return (String^)(header->Get("first_class")); }
        }
        virtual property List<ExplorerNode^>^ Children
        {
            List<ExplorerNode^>^ get() override { return noChildren; }
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
        List<ExplorerNode^>^ tags;
    public:
        HaloTagClassNode(String^ className, List<ExplorerNode^>^ tags)
        {
            this->className = className;
            this->tags = tags;
        }
        virtual property String^ Name { String^ get() override { return className; } }
        virtual property List<ExplorerNode^>^ Children
        {
            List<ExplorerNode^>^ get() override { return tags; }
        }
    };

    /// Wraps a PyObject known to be a HaloMap
    public ref class HaloMapNode : ExplorerNode
    {
        PyObject* halomap;
        List<ExplorerNode^>^ tagClasses{};

    public:
        HaloMapNode(PyObject* map);
        HaloTagNode^ getGhost();
        virtual property List<ExplorerNode^>^ Children
        {
            List<ExplorerNode^>^ get() override { return tagClasses; }
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
        property List<ExplorerNode^>^ maps;

        bool initialized = false;

    public:
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %instance; } }
        void OpenMap(HaloMemory whichExe);
        void OpenMap(String^ filename);
        virtual property List<ExplorerNode^>^ Children
        {
            List<ExplorerNode^>^ get() { return maps; }
        }
        virtual String^ ToString() override;
    };
}
