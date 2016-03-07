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

    private:
        PyObject* _po;
    };

    /// Callback function to be passed into Python.
    int callback(BoundPyObject* slf);

#pragma managed(pop)

    public ref class HaloMapProxy
    {
    public:
        /// Pseudo-main function for developing purposes.
        static void doThing()
        {
            Py_Initialize();
            PyRun_SimpleString(
                // Fix console output
                "import sys\n"
                "sys.stdout = open('CONOUT$', 'wt')");

            // Load map
            PyRun_SimpleString(
                "import halolib\n"
                "map = halolib.HaloMap.from_hpc()\n"
                "ghost = map.tag('vehi', 'rwarthog')\n"
                "print(repr(ghost))");

            PyObject* sys_mod_dict = PyImport_GetModuleDict();
            PyObject* main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
            PyObject* ghost = PyObject_GetAttrString(main_mod, "ghost");
            PyObject* ghostdata = PyObject_GetAttrString(ghost, "data");

            BoundPyObject* bpo = new BoundPyObject(ghostdata);

            int result_a = PyObject_SetAttr(ghost, PyUnicode_FromString("acceleration"), PyFloat_FromDouble(12));
            int result_b = PyObject_SetAttr(ghost, PyUnicode_FromString("max_forward_velocity"), PyFloat_FromDouble(12));
            //PyRun_SimpleString("map.index_header.integrity = 'n00b'");

            PyRun_SimpleString("print(repr(ghost))"); 
            PyRun_SimpleString("print('exiting')");
            Py_Finalize();
        }
    };
}
