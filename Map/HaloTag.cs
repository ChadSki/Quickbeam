using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Windows;

namespace Quickbeam
{
    class HaloTag
    {
        private GCHandle _pinnedHaloMap;

        public string FirstClass { get; private set; }
        public string SecondClass { get; private set; }
        public string ThirdClass { get; private set; }
        public int Ident { get; private set; }
        public int NameOffsetRaw { get; private set; }
        public int NameOffset { get; private set; }
        public int MetaOffsetRaw { get; private set; }
        public int MetaOffset { get; private set; }
        public int NextTagMetaOffset { get; set; }
        public int Indexed { get; private set; }
        public string Name { get; private set; }
        private IntPtr Meta { get; set; }

        public HaloTag(HaloTagStruct hts, int mapMagic, GCHandle pinnedHaloMap)
        {
            _pinnedHaloMap = pinnedHaloMap;

            FirstClass = StringUtil.Reverse(hts.firstClass);
            SecondClass = StringUtil.Reverse(hts.secondClass);
            ThirdClass = StringUtil.Reverse(hts.thirdClass);
            Ident = hts.ident;
            NameOffsetRaw = hts.nameOffsetRaw;
            NameOffset = hts.nameOffsetRaw - mapMagic;
            MetaOffsetRaw = hts.metaOffsetRaw;
            MetaOffset = hts.metaOffsetRaw - mapMagic;
            Indexed = hts.indexed;

            var addrOfName = (_pinnedHaloMap.AddrOfPinnedObject() + NameOffset);
            Name = Marshal.PtrToStringAnsi(addrOfName);
        }

        internal void readMeta(ICollection<int> identList, HaloMap theMap)
        {
        }
        
    }
}
