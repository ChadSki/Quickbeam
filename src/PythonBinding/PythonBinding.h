// PythonBinding.h

#pragma once
#include "Stdafx.h"

using namespace System;

namespace PythonBinding {

#pragma managed(push, off)

    /// Wraps a PyObject known to have a property_changed Event.
    public class BoundPyObject
    {
    public:
        BoundPyObject(PyObject* po);
        BoundPyObject(const BoundPyObject &po);
        ~BoundPyObject();

        /// Triggered whenever a property of the bound PyObject
        /// is updated.
        void OnPropertyChanged();

        PyObject* _po;
    };

    /// Callback function to be passed into Python.
    int callback(BoundPyObject* slf);

#pragma managed(pop)

    public ref class HaloMapProxy
    {
    public:
        HaloMapProxy()
        {
            Py_Initialize();
            PyRun_SimpleString( // Fix console output
                "import sys\n"
                "sys.stdout = open('CONOUT$', 'wt')");

            PyRun_SimpleString( // Load map
                "import halolib\n"
                "map = halolib.HaloMap.from_hpc()");

            PyObject* sys_mod_dict = PyImport_GetModuleDict();
            PyObject* main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
            PyObject* map = PyObject_GetAttrString(main_mod, "map");
            this->halomap = new BoundPyObject(map);
        }

        /// Pseudo-main function for developing purposes.
        void doThing()
        {
            //int result_a = PyObject_SetAttr(ghost, PyUnicode_FromString("acceleration"), PyFloat_FromDouble(12));
            //int result_b = PyObject_SetAttr(ghost, PyUnicode_FromString("max_forward_velocity"), PyFloat_FromDouble(12));
            //PyRun_SimpleString("map.index_header.integrity = 'n00b'");

            PyRun_SimpleString("print(map)"); 
            PyRun_SimpleString("print('exiting')");
            Py_Finalize();
        }
    private:
        BoundPyObject* halomap;
    };
}
