﻿// TODO - I haven't written copy constructors or
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

    public enum class FieldType {
        String, FloatingPoint, Integer, Other,
        };

    /// Wraps a PyObject known to be a HaloStruct
    public ref class HaloStructProxy
    {
        ObservablePyObject* halostruct;
        Dictionary<String^, String^>^ fields;
        Dictionary<String^, FieldType>^ fieldTypes;

    public:
        HaloStructProxy(PyObject* halostruct);
        property Dictionary<String^, String^>^ Fields {
            Dictionary<String^, String^>^ get() { return fields; }
        }
        property Dictionary<String^, FieldType>^ FieldTypes {
            Dictionary<String^, FieldType>^ get() { return fieldTypes; }
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
    public ref class HaloTagProxy : ExplorerNode
    {
        PyObject* halotag;
        HaloStructProxy^ header;
        HaloStructProxy^ data;
        List<ExplorerNode^>^ noChildren{};  // Should remain empty

    public:
        HaloTagProxy(PyObject* halotag);
        property HaloStructProxy^ Header { HaloStructProxy^ get() { return header; } }
        property HaloStructProxy^ Data { HaloStructProxy^ get() { return data; } }
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

    public ref class HaloTagClassProxy : ExplorerNode
    {
        String^ className;
        List<ExplorerNode^>^ tags;
    public:
        HaloTagClassProxy(String^ className, List<ExplorerNode^>^ tags)
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
    public ref class HaloMapProxy : ExplorerNode
    {
        PyObject* halomap;
        List<ExplorerNode^>^ tagClasses{};

    public:
        HaloMapProxy(PyObject* map);
        HaloTagProxy^ getGhost();
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
    public ref class PythonInterpreter : ExplorerNode
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
            List<ExplorerNode^>^ get() override { return maps; }
        }
        virtual property String^ Name { String^ get() override { return "Quickbeam"; } }
        virtual property String^ Suffix { String^ get() override { return "Python Environment"; } }
        virtual String^ ToString() override;
    };
}
