// This is the main DLL file.

#include "stdafx.h"

#include "PythonBinding.h"

namespace PythonBinding {

    PyObject_Thunk::PyObject_Thunk(PyObject* po)
        : _po(po)
    {
        PyObject* result0 = PyObject_CallMethod(po, "register_callback", "KK", // (uint64, uint64)
            reinterpret_cast<UINT64>(&callback),
            reinterpret_cast<UINT64>(this));
    }

    PyObject_Thunk::~PyObject_Thunk()
    {
        // TODO
    }

    void PyObject_Thunk::OnPropertyChanged()
    {
        std::cout << "changed! (C++ OnPropertyChanged)" << std::endl;
    }

    int callback(PyObject_Thunk* slf)
    {
        slf->OnPropertyChanged();
        return 0;
    }

    HaloStructProxy::HaloStructProxy()
    {}

    HaloStructProxy::HaloStructProxy(const HaloStructProxy^ & other)
    {}

    HaloStructProxy::~HaloStructProxy()
    {}

    HaloTagProxy::HaloTagProxy(PyObject* tag)
    {
        this->tag = tag;
    }

    HaloTagProxy::HaloTagProxy(const HaloTagProxy^ & other)
    {
        this->tag = other->tag;
    }

    HaloTagProxy::~HaloTagProxy()
    {}

    HaloMapProxy::HaloMapProxy()
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
        this->halomap = new PyObject_Thunk(map);
    }

    HaloMapProxy::HaloMapProxy(const HaloMapProxy^ & other)
    {}

    HaloMapProxy::~HaloMapProxy()
    {
        PyRun_SimpleString("print('exiting')");
        Py_Finalize();
    }

    HaloTagProxy^ HaloMapProxy::getGhost()
    {

        //int result_a = PyObject_SetAttr(ghost, PyUnicode_FromString("acceleration"), PyFloat_FromDouble(12));
        //int result_b = PyObject_SetAttr(ghost, PyUnicode_FromString("max_forward_velocity"), PyFloat_FromDouble(12));
        //PyRun_SimpleString("map.index_header.integrity = 'n00b'");

        PyRun_SimpleString("ghost = map.tag('vehi', 'ghost')");
        PyObject* sys_mod_dict = PyImport_GetModuleDict();
        PyObject* main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
        PyObject* ghost = PyObject_GetAttrString(main_mod, "ghost");

        return gcnew HaloTagProxy(ghost);
    }
}
