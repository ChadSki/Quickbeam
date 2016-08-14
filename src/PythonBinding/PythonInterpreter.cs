using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PythonBinding
{
    public class PythonInterpreter
    {
        public static PythonInterpreter Instance = new PythonInterpreter();

        private PyObj mainModule = null;

        internal Dictionary<IntPtr, PyObj> ObjectCache = new Dictionary<IntPtr, PyObj>();

        private void initializeEnvironment()
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
        public PyObj MainModule
        {
            get
            {
                if (mainModule == null) initializeEnvironment();
                return mainModule;
            }
        }

        /// Execute a string as a Python script.
        public void RunSimpleString(string script)
        {
            if (mainModule == null) initializeEnvironment();
            CPython.PyRun_SimpleString(script);
        }
    }
}
