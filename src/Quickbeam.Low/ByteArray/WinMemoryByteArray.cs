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
using Microsoft.Win32.SafeHandles;

namespace Quickbeam.Low.ByteArray
{
    /// <summary>
    /// Builder for creating multiple WinMemoryByteArrays targeting the same process.
    /// </summary>
    public class WinMemoryByteArrayBuilder : IByteArrayBuilder, IDisposable
    {
        private readonly SafeWaitHandle _processHandle;

        public WinMemoryByteArrayBuilder(string processName)
        {
            var processesByName = Process.GetProcessesByName(processName);
            if (processesByName.Length > 0)
            {
                _processHandle = new SafeWaitHandle(NativeMethods.OpenProcess(NativeMethods.ProcessAllAccess, false, processesByName[0].Id), true);
            }
            else
            {
                throw new ArgumentException("Process is not running.");
            }
        }

        public IByteArray CreateByteArray(int offset, int size)
        {
            return new WinMemoryByteArray(offset, size, _processHandle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _processHandle.Close();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Implements IByteArray for regions of another process' memory.
    /// </summary>
    /// <remarks>
    /// Instantiate using WinMemoryByteArrayBuilder.
    /// </remarks>
    public class WinMemoryByteArray : BaseByteArray
    {
        private readonly SafeWaitHandle _processHandle;

        internal WinMemoryByteArray(int offset, int size, SafeWaitHandle processHandle)
            : base(offset, size)
        {
            _processHandle = processHandle;
        }

        override protected byte[] ReadBytesCore(int offset, int length)
        {
            var buf = new byte[length];
            int bytesWritten;
            NativeMethods.ReadProcessMemory(_processHandle.DangerousGetHandle(), (IntPtr)(Offset + offset), buf, length, out bytesWritten);
            return buf;
        }

        override protected void WriteBytesCore(int offset, byte[] data)
        {
            int bytesWritten;
            NativeMethods.WriteProcessMemory(_processHandle.DangerousGetHandle(), (IntPtr)(Offset + offset), data, data.Length, out bytesWritten);
        }
    }
}
