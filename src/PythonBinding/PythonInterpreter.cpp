#include "stdafx.h"
#include "PythonInterpreter.h"

PythonBinding::PythonInterpreter::PythonInterpreter()
{
    Py_Initialize();
    PyRun_SimpleString(
        "import sys\n"
        "sys.stdout = open('CONOUT$', 'wt')\n"  // Fix console output
        "import halolib\n");

    PyObject* sys_mod_dict = PyImport_GetModuleDict();
    PyObject* main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
    this->halolib = PyObject_GetAttrString(main_mod, "halolib");
}

PythonBinding::HaloMapProxy ^ PythonBinding::PythonInterpreter::OpenMap(HaloMemory whichExe)
{
    auto halolib_dict = PyModule_GetDict(halolib);
    auto halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
    PyObject* map_constructor{};
    switch (whichExe)
    {
    case HaloMemory::PC:
        std::cout << "PC" << std::endl;
        map_constructor = PyObject_GetAttrString(halomap_class, "from_hpc");
        break;

    case HaloMemory::CE:
        std::cout << "CE" << std::endl;
        map_constructor = PyObject_GetAttrString(halomap_class, "from_hce");
        break;
    }
    std::cout << "map_constructor " << map_constructor << std::endl;
    auto map = PyObject_CallObject(map_constructor, nullptr);
    std::cout << "map " << map << std::endl;
    return gcnew PythonBinding::HaloMapProxy(map);
}

PythonBinding::HaloMapProxy ^ PythonBinding::PythonInterpreter::OpenMap(System::String ^ filename)
{
    throw gcnew System::NotImplementedException();
    // TODO: insert return statement here
}
