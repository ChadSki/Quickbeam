// PythonBinding.h

#pragma once
#include "Stdafx.h"

using namespace System;

namespace PythonBinding {

    /*
    /// Wraps a PyObject known to have a property_changed Event.
    public class BoundPyObject
    {
    public:
        BoundPyObject(PyObject* po);
        BoundPyObject(const BoundPyObject &po);
        ~BoundPyObject();

        /// Triggered whenever a property of the bound PyObject
        /// is updated.
        void OnPropertyChanged(char* name);

    private:
        PyObject* _po;
    };*/

    public ref class Class1
    {
    public:
        /// Pseudo-main function for developing purposes.
        ///
        static void doThing()
        {
            Py_Initialize();
            PyRun_SimpleString("import sys");
            PyRun_SimpleString("sys.stdout = open('CONOUT$', 'wt')");
            PyRun_SimpleString("print('hello')");
            Py_Finalize();
        }
    };
}
