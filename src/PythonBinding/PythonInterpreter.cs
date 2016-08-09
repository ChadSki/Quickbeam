using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PythonBinding
{
    public class PythonInterpreter
    {
        private static PyObj mainModule = null;

        internal static Dictionary<IntPtr, PyObj> ObjectCache = new Dictionary<IntPtr, PyObj>();

        private static void initializeEnvironment()
        {
            CPython.Py_SetProgramName(Process.GetCurrentProcess().MainModule.FileName);
            CPython.Py_Initialize();
            var exitCode = CPython.PyRun_SimpleString(CPython.StartupScript);
            if (exitCode == -1) throw new Exception("PyRun_SimpleString did not execute successfully.");
            unsafe
            {
                var sysModDict = CPython.PyImport_GetModuleDict();
                var rawMainModule = CPython.PyMapping_GetItemString(sysModDict, "__main__");
                mainModule = PyObj.FromPointer(rawMainModule);
            }
        }

        /// The main Python module. Singleton property.
        public static PyObj MainModule
        {
            get
            {
                if (mainModule == null) initializeEnvironment();
                return mainModule;
            }
        }

        /// Execute a string as a Python script.
        public static void RunSimpleString(string script)
        {
            if (mainModule == null) initializeEnvironment();
            CPython.PyRun_SimpleString(script);
        }
    }
}
