using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Quickbeam
{
    /// <summary>
    /// Represents the structure of a binary Halo 1 map header
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    struct MapHeaderStruct
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string integrity;

        [FieldOffset(4)]
        public int gameVersion;

        [FieldOffset(8)]
        public int mapSize;

        // Skip 4 bytes

        [FieldOffset(16)]
        public int indexOffset;

        [FieldOffset(20)]
        public int metaDataLength;

        // Skip 8 bytes

        [FieldOffset(32)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string mapName;

        [FieldOffset(64)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string mapBuild;

        [FieldOffset(96)]
        public int mapType;
    }
}
