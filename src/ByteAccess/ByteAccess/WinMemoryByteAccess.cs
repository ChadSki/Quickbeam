using System;
using System.Diagnostics;

namespace Quickbeam.Low.ByteAccess
{
    /// <summary>
    /// Builder for creating multiple WinMemoryByteAccesses targeting the same process.
    /// </summary>
    public class WinMemoryByteAccessBuilder : IByteAccessBuilder
    {
        private readonly IntPtr _processId = IntPtr.Zero;

        public WinMemoryByteAccessBuilder(string processName)
        {
            var processesByName = Process.GetProcessesByName(processName); // "halo"
            if (processesByName.Length > 0)
            {
                _processId = NativeMethods.OpenProcess(NativeMethods.ProcessAllAccess, false, processesByName[0].Id);
            }
            else
            {
                throw new Exception();
            }
        }

        public IByteAccess CreateByteAccess(int offset, int size)
        {
            return new WinMemoryByteAccess(offset, size, _processId);
        }
    }

    /// <summary>
    /// Encapsulates mapfile access to a specific area of bytes in a process' memory.
    /// </summary>
    public class WinMemoryByteAccess : BaseByteAccess
    {
        private readonly IntPtr _processId = IntPtr.Zero;

        public WinMemoryByteAccess(int beginAccessOffset, int accessSize, IntPtr processId)
            : base(beginAccessOffset, accessSize)
        {
            _processId = processId;
        }

        override protected byte[] ReadBytesImpl(int offset, int length)
        {
            var buf = new byte[length];
            int bytesWritten;
            NativeMethods.ReadProcessMemory(_processId, (IntPtr)(Offset + offset), buf, length, out bytesWritten);
            return buf;
        }

        override protected void WriteBytesImpl(int offset, byte[] data)
        {
            int bytesWritten;
            NativeMethods.WriteProcessMemory(_processId, (IntPtr)(Offset + offset), data, data.Length, out bytesWritten);
        }
    }
}
