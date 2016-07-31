using System;

namespace NimbusSharp
{
    public class PyObj
    {
        unsafe private CPython.PyObject* obj;
        unsafe internal PyObj(CPython.PyObject* obj)
        {
            this.obj = obj;
        }

        public PyObj CallObject(PyObj args) { throw new NotImplementedException(); }
        public PyObj GetAttrString(string attrName) { throw new NotImplementedException(); }
        public PyObj GetItem(PyObj key) { throw new NotImplementedException(); }
        public PyObj GetIter() { throw new NotImplementedException(); }

        public override string ToString()
        {
            string str;
            unsafe
            {
                var asStr = CPython.PyObject_Str(obj);
                str = CPython.PyUnicode_AsWideCharString(asStr, IntPtr.Zero);
            }
            return str;
        }
        
    }
}
