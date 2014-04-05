// Copyright (c) 2013, Chad Zawistowski
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
