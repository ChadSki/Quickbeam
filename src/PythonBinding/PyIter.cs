using System;

namespace PythonBinding
{
    public class PyIter : PyObj
    {
        unsafe internal PyIter(CPython.PyObject* obj) : base(obj) {}

        public PyObj Next()
        {
            PyObj result;
            unsafe
            {
                var rawResult = CPython.PyIter_Next(obj);
                if (rawResult == null) throw new NullReferenceException("PyIter refused to yield an item.");
                result = new PyObj(rawResult);
            }
            return result;
        }
    }
}
