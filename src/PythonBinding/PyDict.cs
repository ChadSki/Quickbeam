using System;

namespace PythonBinding
{
    public class PyDict : PyObj
    {
        unsafe internal PyDict(CPython.PyObject* obj) : base(obj) { }

        /// Get an item from the dict by its key.
        public new PyObj GetItem(PyObj key)
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyDict_GetItem(obj, key.obj);
                if (rawResult == null) throw new NullReferenceException("Failed to get item by key.");
                result = new PyObj(rawResult);
            }
            return result;
        }
    }
}
