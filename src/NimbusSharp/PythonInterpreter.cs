using System;
using System.Diagnostics;

namespace NimbusSharp
{
    public class PythonInterpreter
    {
        private static PyModule mainModule = null;

        /// The main Python module. Singleton property, initializes Python environment on first access.
        public static PyModule MainModule
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
                        var rawMainModule = CPython.PyMapping_GetItemString(sysModDict, "__main__");
                        mainModule = new PyModule(rawMainModule);
                    }
                }
                return mainModule;
            }
        }
    }
}
