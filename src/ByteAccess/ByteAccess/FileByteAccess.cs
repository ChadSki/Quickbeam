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

using System.IO;
using System.IO.MemoryMappedFiles;

namespace Quickbeam.Low.ByteAccess
{
    /// <summary>
    /// Builder for creating multiple FileByteAccesses targeting the same file.
    /// </summary>
    public class FileByteAccessBuilder : IByteAccessBuilder
    {
        private readonly MemoryMappedFile _handle;

        public FileByteAccessBuilder(string filepath)
        {
            _handle = MemoryMappedFile.CreateFromFile(filepath, FileMode.Open);
        }

        public IByteAccess CreateByteAccess(int offset, int size)
        {
            return new FileByteAccess(offset, size, _handle.CreateViewAccessor(offset, size));
        }
    }

    public class FileByteAccess : BaseByteAccess
    {
        private readonly MemoryMappedViewAccessor _f;

        public FileByteAccess(int offset, int size, MemoryMappedViewAccessor f)
            : base(offset, size)
        {
            _f = f;
        }

        override protected byte[] ReadBytesImpl(int offset, int length)
        {
            var bytes = new byte[length];
            _f.ReadArray(0, bytes, offset, length);
            return bytes;
        }

        override protected void WriteBytesImpl(int offset, byte[] data)
        {
            _f.WriteArray(0, data, offset, data.Length);
        }

        // MemoryMappedViewAccessor directly supports these, so override the slower BaseByteAccess workarounds.

        public new float ReadFloat32(int offset) { return _f.ReadSingle(offset); }
        public new double ReadFloat64(int offset) { return _f.ReadDouble(offset); }
        public new sbyte ReadInt8(int offset) { return _f.ReadSByte(offset);  }
        public new short ReadInt16(int offset) { return _f.ReadInt16(offset);  }
        public new int ReadInt32(int offset) { return _f.ReadInt32(offset);  }
        public new long ReadInt64(int offset) { return _f.ReadInt64(offset);  }
        public new byte ReadUInt8(int offset) { return _f.ReadByte(offset); }
        public new ushort ReadUInt16(int offset) { return _f.ReadUInt16(offset); }
        public new uint ReadUInt32(int offset) { return _f.ReadUInt32(offset); }
        public new ulong ReadUInt64(int offset) { return _f.ReadUInt64(offset); }

        public new void WriteFloat32(int offset, float toWrite) { _f.Write(offset, toWrite); }
        public new void WriteFloat64(int offset, double toWrite) { _f.Write(offset, toWrite); }
        public new void WriteInt8(int offset, sbyte toWrite) { _f.Write(offset, toWrite); }
        public new void WriteInt16(int offset, short toWrite) { _f.Write(offset, toWrite); }
        public new void WriteInt32(int offset, int toWrite) { _f.Write(offset, toWrite); }
        public new void WriteInt64(int offset, long toWrite) { _f.Write(offset, toWrite); }
        public new void WriteUInt8(int offset, byte toWrite) { _f.Write(offset, toWrite); }
        public new void WriteUInt16(int offset, ushort toWrite) { _f.Write(offset, toWrite); }
        public new void WriteUInt32(int offset, uint toWrite) { _f.Write(offset, toWrite); }
        public new void WriteUInt64(int offset, ulong toWrite) { _f.Write(offset, toWrite); }
    }
}
