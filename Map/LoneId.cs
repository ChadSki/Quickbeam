using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Quickbeam
{
    [StructLayout(LayoutKind.Sequential)]
    struct LoneIdStruct
    {
        public int refIdent;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FourBytesStruct
    {
        public byte one;
        public byte two;
        public byte three;
        public byte four;
    }
}
