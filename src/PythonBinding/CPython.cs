using System;
using System.Runtime.InteropServices;

namespace PythonBinding
{
    internal static class CPython
    {
        private const string pythonDll = @"python35\python35.dll";

        /// This snippet needs to be run in order to enable Windows console output.
        internal const string StartupScript =
@"import sys
#sys.stdout = sys.stderr = open('CONOUT$', 'wt')
sys.stdout = sys.stderr = open('C:\embedded_log_file.txt', 'w')
print('Python initialized.')";

        internal struct PyObject { }

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_DecRef(IntPtr obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_IncRef(IntPtr obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_Initialize();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Py_SetProgramName([MarshalAs(UnmanagedType.LPWStr)]string name);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyDict_GetItem(IntPtr dict, IntPtr key);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PyErr_Clear();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyErr_Occurred();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PyErr_Print();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern  double PyFloat_AsDouble(IntPtr value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyFloat_FromDouble(double value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyImport_GetModuleDict();

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyIter_Next(IntPtr iterator);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long PyLong_AsLong(IntPtr value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyLong_FromLong(long value);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyMapping_GetItemString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)]string key);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyModule_GetDict(IntPtr module);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyObject_CallObject(IntPtr callable, IntPtr args);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyObject_GetAttrString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)]string attrName);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyObject_GetItem(IntPtr obj, IntPtr key);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyObject_GetIter(IntPtr obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyObject_Str(IntPtr obj);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyObject_Print(IntPtr toPrint, IntPtr filestreamHandle, int mode);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyRun_SimpleString([MarshalAs(UnmanagedType.LPStr)]string toRun);

        // Don't forget to wrap variadic things in `__arglist(...)`
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyTuple_Pack(IntPtr size, __arglist);

        // TODO - fix leak and null
        // Returns a buffer allocated by PyMem_Alloc() (use PyMem_Free() to free it) on success.
        // Note that the resulting wchar_t string might contain null characters, which would cause the string to be truncated when used with most C functions.
        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        internal static extern string PyUnicode_AsWideCharString(IntPtr str, IntPtr size);

        [DllImport(pythonDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyUnicode_FromString([MarshalAs(UnmanagedType.LPStr)]string str);
    }
}
