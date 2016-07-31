using System;

namespace PythonBinding
{
    public class PyObj
    {
        unsafe internal CPython.PyObject* obj;

        /// These should only be constructed by APIs in this assembly.
        unsafe internal PyObj(CPython.PyObject* obj)
        {
            CPython.Py_IncRef(obj);
            this.obj = obj;
        }

        unsafe ~PyObj()
        {
            CPython.Py_DecRef(obj);
        }

        /// Call this Python object with the given arguments.
        public PyObj CallObject(PyObj args)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyObject_CallObject(obj, args.obj);
                if (rawResult == null) throw new NullReferenceException("Calling PyObject returned null.");
                result = new PyObj(rawResult);
            }
            return result;
        }

        /// Get an attribute from this PyObject by string name.
        public PyObj GetAttrString(string attrName)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyObject_GetAttrString(obj, attrName);
                if (rawResult == null) throw new NullReferenceException(
                    string.Format("Failed to get attribute `{0}` from PyObject.", attrName));
                result = new PyObj(rawResult);
            }
            return result;
        }

        /// If this Python object supports [] indexing, get an item by its key.
        public PyObj GetItem(PyObj key)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyObject_GetItem(obj, key.obj);
                if (rawResult == null) throw new NullReferenceException("Failed to get item by key.");
                result = new PyObj(rawResult);
            }
            return result;
        }

        /// If this Python object is iterable, get the iterator.
        public PyIter GetIter()
        {
            PyIter result;
            unsafe
            {
                var rawResult = CPython.PyObject_GetIter(obj);
                if (rawResult == null) throw new NullReferenceException("Failed iterate over PyObject.");
                result = new PyIter(rawResult);
            }
            return result;
        }

        /// Double float representation of this PyObject.
        public double ToDouble()
        {
            double result;
            unsafe { result = CPython.PyFloat_AsDouble(obj); }
            return result;
        }

        /// String representation of this PyObject.
        public override string ToString()
        {
            string str;
            unsafe
            {
                var asStr = CPython.PyObject_Str(obj);
                if (asStr == null) throw new NullReferenceException("Failed to convert PyObject to string.");
                str = CPython.PyUnicode_AsWideCharString(asStr, IntPtr.Zero);
            }
            return str;
        }

        /// Long integer representation of this PyObject.
        public long ToLong()
        {
            long result;
            unsafe { result = CPython.PyLong_AsLong(obj); }
            return result;
        }

        /// Create a PyObject from a double.
        public static PyObj FromDouble(double value)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyFloat_FromDouble(value);
                if (rawResult == null) throw new NullReferenceException("Failed to create PyObject from double.");
                result = new PyObj(rawResult);
            }
            return result;
        }

        /// Create a PyObject from a long integer.
        public static PyObj FromLong(long value)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyLong_FromLong(value);
                if (rawResult == null) throw new NullReferenceException("Failed to create PyObject from long.");
                result = new PyObj(rawResult);
            }
            return result;
        }
    }
}
