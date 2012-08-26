using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Quickbeam
{
    /// <summary>
    /// Represents the 16 bytes which make up a dependency. Dependencies
    /// appear in a tag's meta.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    struct ReferenceStruct
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string refClass;

        [FieldOffset(4)]
        public int refNamePtr;

        [FieldOffset(8)]
        public int zeros;

        [FieldOffset(12)]
        public int refIdent;
    }
}
