using System;

namespace NimbusSharp
{
    public class PyModule : PyObj
    {
        unsafe internal PyModule(CPython.PyObject* obj) : base(obj) { }

        /// Get a member of this module by name.
        public PyObj GetMember(string attrName)
        {
            PyObj result;
            unsafe
            {
                var rawDict = CPython.PyModule_GetDict(obj);
                if (rawDict == null) throw new NullReferenceException("Failed to get dict from module.");
                var rawString = CPython.PyUnicode_FromString(attrName);
                if (rawString == null) throw new NullReferenceException("Failed to create PyObject from string.");
                var rawResult = CPython.PyDict_GetItem(rawDict, rawString);
                if (rawResult == null) throw new NullReferenceException("Failed to get item by key.");
                result = new PyObj(rawResult);
            }
            return result;
        }
    }
}
