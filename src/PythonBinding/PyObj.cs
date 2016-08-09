using System;

namespace PythonBinding
{
    public class PyObj
    {
        unsafe internal CPython.PyObject* obj;

        /// Increment the PyObject's reference count and save
        /// its address.
        unsafe private PyObj(CPython.PyObject* obj)
        {
            // TODO this only needs to be incremented if the manual
            // says it is providing a borrowed reference, which is the
            // uncommon case.
            CPython.Py_IncRef(obj);
            this.obj = obj;
        }

        /// Tell Python when we're finished referring to this object.
        unsafe ~PyObj()
        {
            PythonInterpreter.ObjectCache.Remove((IntPtr)obj);
            CPython.Py_DecRef(obj);
        }

        /// Create a new C# wrapper, or return the existing wrapper
        unsafe internal static PyObj FromPointer(CPython.PyObject* obj)
        {
            var id = (IntPtr)obj;
            PyObj result;
            if (PythonInterpreter.ObjectCache.TryGetValue(id, out result))
            {
                result = new PyObj(obj);
                PythonInterpreter.ObjectCache[id] = result;
            }
            return result;
        }

        /// Call this Python object with the given arguments.
        public PyObj Call(PyObj args = null)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyObject_CallObject(obj, args.obj);
                if (rawResult == null) throw new NullReferenceException("Calling PyObject returned null.");
                result = FromPointer(rawResult);
            }
            return result;
        }

        /// Get an attribute from this PyObject by string name.
        public PyObj Attr(string attrName)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyObject_GetAttrString(obj, attrName);
                if (rawResult == null) throw new NullReferenceException(
                    string.Format("Failed to get attribute `{0}` from PyObject.", attrName));
                result = FromPointer(rawResult);
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
                result = FromPointer(rawResult);
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
                result = (PyIter)FromPointer(rawResult);
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
                result = FromPointer(rawResult);
            }
            return result;
        }

        /// Create a PyObject from a string.
        public static PyObj FromString(string value)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyUnicode_FromString(value);
                if (rawResult == null) throw new NullReferenceException("Failed to create PyObject from string.");
                result = FromPointer(rawResult);
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
                result = FromPointer(rawResult);
            }
            return result;
        }

        public class PyIter : PyObj
        {
            unsafe internal PyIter(CPython.PyObject* obj) : base(obj) { }

            public PyObj Next()
            {
                PyObj result;
                unsafe
                {
                    var rawResult = CPython.PyIter_Next(obj);
                    if (rawResult == null) throw new NullReferenceException("PyIter refused to yield an item.");
                    result = FromPointer(rawResult);
                }
                return result;
            }
        }
    }
}
