#include "stdafx.h"
#include "PythonInterpreter.h"

PythonBinding::PythonInterpreter::PythonInterpreter()
{
    Py_Initialize();
    PySys_SetPath(L".");
    PyRun_SimpleString(
        "import sys\n"
        "import halolib\n"
        "sys.stdout = open('CONOUT$', 'wt')\n");  // Fix console output
}

PythonBinding::HaloMapProxy ^ PythonBinding::PythonInterpreter::OpenMap(HaloMemory whichExe)
{
    auto halolib = PyImport_ImportModule("halolib");
    auto halolib_dict = PyModule_GetDict(halolib);
    auto halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
    PyObject* map_constructor;
    switch (whichExe)
    {
    case HaloMemory::PC:
        map_constructor = PyObject_GetAttrString(halomap_class, "from_hpc");

    case HaloMemory::CE:
        map_constructor = PyObject_GetAttrString(halomap_class, "from_hce");
    }
    return gcnew PythonBinding::HaloMapProxy(PyObject_CallObject(map_constructor, nullptr));
}

PythonBinding::HaloMapProxy ^ PythonBinding::PythonInterpreter::OpenMap(System::String ^ filename)
{
    throw gcnew System::NotImplementedException();
    // TODO: insert return statement here
}
