using System;

namespace PythonBinding
{
    public class PyObj
    {
        internal IntPtr obj;

        private PyObj(IntPtr obj)
        {
            this.obj = obj;
        }

        /// Tell Python when we're finished referring to this object.
        ~PyObj()
        {
            PythonInterpreter.Instance.ObjectCache.Remove((IntPtr)obj);
            CPython.Py_DecRef(obj);
        }

        /// Create a new C# wrapper, or return the existing wrapper
        internal static PyObj FromPointer(IntPtr objPtr, bool needsIncRef = false)
        {
            // Unlikely to need to do this, but just in case somewhere does.
            if (needsIncRef) CPython.Py_IncRef(objPtr);

            PyObj result;
            if (!PythonInterpreter.Instance.ObjectCache.TryGetValue(objPtr, out result))
            {
                result = new PyObj(objPtr);
                PythonInterpreter.Instance.ObjectCache[objPtr] = result;
            }
            return result;
        }

        /// Call this Python object with the given arguments.
        public PyObj Call(PyObj argsTuple = null)
        {
            PyObj result;
            var args = (argsTuple == null) ? IntPtr.Zero : argsTuple.obj;
            var rawResult = CPython.PyObject_CallObject(obj, args);
            if (rawResult == IntPtr.Zero)
            {
                PrintPythonException();
                throw new NullReferenceException("Calling PyObject returned null.");
            }
            result = FromPointer(rawResult);
            return result;
        }

        /// Get an attribute from this PyObject by string name.
        public PyObj Attr(string attrName)
        {
            var rawResult = CPython.PyObject_GetAttrString(obj, attrName);
            if (rawResult == IntPtr.Zero) throw new NullReferenceException(
                string.Format("Failed to get attribute `{0}` from PyObject.", attrName));
            return FromPointer(rawResult);
        }

        /// If this Python object supports [] indexing, get an item by its key.
        public PyObj GetItem(PyObj key)
        {
            var rawResult = CPython.PyObject_GetItem(obj, key.obj);
            if (rawResult == IntPtr.Zero) throw new NullReferenceException("Failed to get item by key.");
            return FromPointer(rawResult);
        }

        /// If this Python object is iterable, get the iterator.
        public PyObj GetIter()
        {
            var rawResult = CPython.PyObject_GetIter(obj);
            if (rawResult == IntPtr.Zero) throw new NullReferenceException("Failed iterate over PyObject.");
            return FromPointer(rawResult);
        }

        /// If this Python object is an iterator, get the next item in the sequence.
        public PyObj Next()
        {
            var rawResult = CPython.PyIter_Next(obj);
            return (rawResult == IntPtr.Zero) ? null : FromPointer(rawResult);
        }

        /// Double float representation of this PyObject.
        public double ToDouble()
        {
            return CPython.PyFloat_AsDouble(obj);
        }

        /// String representation of this PyObject.
        public override string ToString()
        {
            var asStr = CPython.PyObject_Str(obj);
            if (asStr == IntPtr.Zero) throw new NullReferenceException("Failed to convert PyObject to string.");
            return CPython.PyUnicode_AsWideCharString(asStr, IntPtr.Zero);
        }

        /// Long integer representation of this PyObject.
        public long ToLong()
        {
            return CPython.PyLong_AsLong(obj);
        }

        /// Create a PyObject from a double.
        public static PyObj FromDouble(double value)
        {
            var rawResult = CPython.PyFloat_FromDouble(value);
            if (rawResult == IntPtr.Zero) throw new NullReferenceException("Failed to create PyObject from double.");
            return FromPointer(rawResult);
        }

        /// Create a PyObject from a string.
        public static PyObj FromString(string value)
        {
            var rawResult = CPython.PyUnicode_FromString(value);
            if (rawResult == IntPtr.Zero) throw new NullReferenceException("Failed to create PyObject from string.");
            return FromPointer(rawResult);
        }

        /// Create a PyObject from a long integer.
        public static PyObj FromLong(long value)
        {
            var rawResult = CPython.PyLong_FromLong(value);
            if (rawResult == IntPtr.Zero) throw new NullReferenceException("Failed to create PyObject from long.");
            return FromPointer(rawResult);
        }

        /// For diagnostic purposes
        private static void PrintPythonException()
        {
            if (CPython.PyErr_Occurred() == IntPtr.Zero)
            {
                throw new InvalidOperationException("Python has not had an exception to handle!");
            }
            else
            {
                CPython.PyErr_Print();
                CPython.PyErr_Clear();
            }
        }
    }
}
