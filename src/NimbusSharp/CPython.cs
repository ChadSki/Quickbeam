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

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void Py_DecRef(PyObject* obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void Py_IncRef(PyObject* obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_Initialize();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_SetProgramName([MarshalAs(UnmanagedType.LPWStr)]string name);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyDict_GetItem(PyObject* dict, PyObject* key);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe  double PyFloat_AsDouble(PyObject* value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyFloat_FromDouble(double value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyImport_GetModuleDict();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyIter_Next(PyObject* iterator);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe long PyLong_AsLong(PyObject* value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyLong_FromLong(long value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyMapping_GetItemString(PyObject* obj, [MarshalAs(UnmanagedType.LPStr)]string key);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyModule_GetDict(PyObject* module);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_CallObject(PyObject* callable, PyObject* args);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_GetAttrString(PyObject* obj, [MarshalAs(UnmanagedType.LPStr)]string attrName);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_GetItem(PyObject* obj, PyObject* key);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_GetIter(PyObject* obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyObject_Str(PyObject* obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int PyObject_Print(PyObject* toPrint, IntPtr filestreamHandle, int mode);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyRun_SimpleString([MarshalAs(UnmanagedType.LPStr)]string toRun);

        // Don't forget to wrap variadic things in `__arglist(...)`
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyTuple_Pack(IntPtr size, __arglist);

        // TODO - fix leak and null
        // Returns a buffer allocated by PyMem_Alloc() (use PyMem_Free() to free it) on success.
        // Note that the resulting wchar_t string might contain null characters, which would cause the string to be truncated when used with most C functions.
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        internal static extern unsafe string PyUnicode_AsWideCharString(PyObject* str, IntPtr size);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe PyObject* PyUnicode_FromString([MarshalAs(UnmanagedType.LPStr)]string str);
    }
}
