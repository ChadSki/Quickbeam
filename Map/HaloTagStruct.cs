using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Quickbeam
{
    /// <summary>
    /// Represents the structure of a binary Halo 1 tag entry as it appears
    /// in the tag index
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    class HaloTagStruct
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string firstClass;

        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string secondClass;

        [FieldOffset(8)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string thirdClass;

        [FieldOffset(12)]
        public int ident;

        [FieldOffset(16)]
        public int nameOffsetRaw;

        [FieldOffset(20)]
        public int metaOffsetRaw;

        [FieldOffset(24)]
        public int indexed;

        [FieldOffset(28)]
        private int unknown; // No known use
    }
}
