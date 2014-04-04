using System;
using System.Runtime.InteropServices;

namespace Quickbeam.ByteAccess.Memory
{
    /// <summary>
    /// Encapsulates mapfile access to a specific area of bytes in memory
    /// </summary>
    public unsafe class WinMemoryByteAccess : BaseByteAccess
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(byte* dst, byte* src, long size);

        /// <summary>
        /// Create a new ByteAccess over a map in Halo's memory
        /// </summary>
        /// <param name="beginAccessOffset">where access begins</param>
        /// <param name="accessSize">how far access extends</param>
        public WinMemoryByteAccess(int beginAccessOffset, int accessSize)
            : base(beginAccessOffset, accessSize)
        {
        }

        override protected byte[] readBytes(int offset, int size)
        {
            byte[] buf = new byte[size];
            var result = HaloInterop.ReadHaloMemory((IntPtr)_baOffset + offset, buf, size);
            return buf;
        }

        override protected void writeBytes(int offset, byte[] data)
        {
            HaloInterop.WriteHaloMemory((IntPtr)_baOffset + offset, data, data.Length);
        }
    }
}
