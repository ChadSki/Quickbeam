using System;

namespace NimbusSharp
{
    public class PyStr : PyObj
    {
        unsafe internal PyStr(CPython.PyObject* obj) : base(obj) { }

        /// Create a PyObject from a string.
        public static PyStr FromString(string value)
        {
            PyStr result;
            unsafe
            {
                var rawResult = CPython.PyUnicode_FromString(value);
                if (rawResult == null) throw new NullReferenceException("Failed to create PyObject from string.");
                result = new PyStr(rawResult);
            }
            return result;
        }
    }
}
