using System;
using System.Runtime.InteropServices;

namespace NimbusSharp
{
    internal static class CPython
    {
        private const string pythonDll = @"python35\python35.dll";
        internal const string StartupScript =
@"import sys
sys.stdout = open('CONOUT$', 'wt')
print('yolo')";

        internal unsafe struct PyObject { }

        //TODO incref/decref?

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_Initialize();

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyDict_GetItem(PyObject* dict, PyObject* key);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyFloat_FromDouble(double value);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyImport_GetModuleDict();

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyIter_Next(PyObject* iterator);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe long PyLong_AsLong(PyObject* value);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyLong_FromLong(long value);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyMapping_GetItemString(PyObject* obj, [MarshalAs(UnmanagedType.LPStr)]string key);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyModule_GetDict(PyObject* module);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_CallObject(PyObject* callable, PyObject* args);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_GetAttrString(PyObject* obj, [MarshalAs(UnmanagedType.LPStr)]string attrName);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_GetItem(PyObject* obj, PyObject* key);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_GetIter(PyObject* obj);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_Str(PyObject* obj);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int PyObject_Print(PyObject* toPrint, IntPtr filestreamHandle, int mode);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyRun_SimpleString([MarshalAs(UnmanagedType.LPStr)]string toRun);

        // TODO test. don't forget to wrap variadic things in `__arglist(...)`
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyTuple_Pack(IntPtr size, __arglist);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal static extern unsafe string PyUnicode_AsUTF8AndSize(PyObject* str, IntPtr size);

        // TODO test
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyUnicode_FromString([MarshalAs(UnmanagedType.LPStr)]string str);
    }
}
