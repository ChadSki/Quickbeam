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

    HaloStructProxy::HaloStructProxy(PyObject* halostruct)
    {
        this->halostruct = halostruct;
    }

    HaloStructProxy::HaloStructProxy(const HaloStructProxy^ & other)
    {
        this->halostruct = other->halostruct;
    }

    HaloStructProxy::~HaloStructProxy()
    {}

    HaloTagProxy::HaloTagProxy(PyObject* halotag)
    {
        this->halotag = halotag;
    }

    HaloTagProxy::HaloTagProxy(const HaloTagProxy^ & other)
    {
        this->halotag = other->halotag;
    }

    HaloTagProxy::~HaloTagProxy()
    {}

    HaloStructProxy^ HaloTagProxy::getData()
    {
        auto data = PyObject_GetAttrString(this->halotag, "data");
        PyObject_Print(data, stdout, Py_PRINT_RAW);
        return gcnew HaloStructProxy(data);
    }

    HaloMapProxy::HaloMapProxy()
    {
        Py_Initialize();
        PyRun_SimpleString( // Fix console output
            "import sys\n"
            "sys.stdout = open('CONOUT$', 'wt')");

        PyRun_SimpleString( // Load map
            "import halolib\n"
            "map = halolib.HaloMap.from_hpc()");

        PySys_SetPath(L".");
        auto halolib = PyImport_ImportModule("halolib");
        auto halolib_dict = PyModule_GetDict(halolib);
        auto halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
        auto from_hpc = PyObject_GetAttrString(halomap_class, "from_hpc");
        this->halomap = PyObject_CallObject(from_hpc, nullptr);
    }

    HaloMapProxy::HaloMapProxy(const HaloMapProxy^ & other)
    {
        this->halomap = other->halomap;
    }

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

        auto tag_fn = PyObject_GetAttrString(this->halomap, "tag");
        auto args = PyTuple_Pack(2,
            PyUnicode_FromString("vehi"),
            PyUnicode_FromString("ghost"));
        auto ghost = PyObject_CallObject(tag_fn, args);
        return gcnew HaloTagProxy(ghost);
    }
}
