#include "stdafx.h"
#include "PythonInterpreter.h"


PythonInterpreter::PythonInterpreter()
{
    Py_Initialize();
    PyRun_SimpleString( // Fix console output
        "import sys\n"
        "sys.stdout = open('CONOUT$', 'wt')");
}
