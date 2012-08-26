using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Quickbeam
{
    /// <summary>
    /// Represents the structure of a binary Halo 1 index header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct IndexHeaderStruct
    {
        public int memoryOffset;
        public int baseTagId;
        public int mapId;
        public int tagCount;
        public int verticieCount;
        public int verticieOffset;
        public int indicieCount;
        public int indicieOffset;
        public int modelDataLength;
        private int unknown;    // unknown use (to align with word boundaries?)
    }
}
