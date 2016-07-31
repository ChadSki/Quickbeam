using System;

namespace NimbusSharp
{
    public class PyObj
    {
        unsafe private CPython.PyObject* obj;
        unsafe internal PyObj(CPython.PyObject* obj)
        {
            this.obj = obj;
        }
    }

    public class PythonInterpreter
    {
        private static PyObj mainModule = null;

        public static PyObj MainModule
        {
            get
            {
                if (mainModule == null)
                {
                    CPython.Py_Initialize();
                    CPython.PyRun_SimpleString(CPython.StartupScript);
                    unsafe {
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
