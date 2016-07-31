using System;
using System.Diagnostics;

namespace NimbusSharp
{
    public class PythonInterpreter
    {
        private static PyObj mainModule = null;

        public static PyObj MainModule
        {
            get
            {
                if (mainModule == null)
                {
                    CPython.Py_SetProgramName(Process.GetCurrentProcess().MainModule.FileName);
                    CPython.Py_Initialize();
                    CPython.PyRun_SimpleString(CPython.StartupScript);
                    unsafe
                    {
                        var sysModDict = CPython.PyImport_GetModuleDict();
                        var mainModuleRaw = CPython.PyMapping_GetItemString(sysModDict, "__main__");
                        mainModule = new PyObj(mainModuleRaw);
                    }
                }
                return mainModule;
            }
        }
    }
}
