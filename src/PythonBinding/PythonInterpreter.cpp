#include "stdafx.h"
#include "PythonInterpreter.h"

PythonBinding::PythonInterpreter::PythonInterpreter()
{
    Py_Initialize();
    PyRun_SimpleString(
        "import sys\n"
        "import halolib\n"
        "sys.stdout = open('CONOUT$', 'wt')\n");  // Fix console output
}

PythonBinding::HaloMapProxy ^ PythonBinding::PythonInterpreter::OpenMap(HaloMemory whichExe)
{
    throw gcnew System::NotImplementedException();
    // TODO: insert return statement here
}

PythonBinding::HaloMapProxy ^ PythonBinding::PythonInterpreter::OpenMap(System::String ^ filename)
{
    throw gcnew System::NotImplementedException();
    // TODO: insert return statement here
}
