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

namespace Quickbeam.Low.ByteAccess
{
    /// <summary>
    /// Builder for creating multiple ByteAccesses targeting the same resource.
    /// </summary>
    public interface IByteAccessBuilder
    {
        IByteAccess CreateByteAccess(int offset, int size);
    }

    /// <summary>
    /// Encapsulates mapfile access to a region of bytes and exposes methods
    /// to serialize or deserialize data to and from that region.
    /// </summary>
    public interface IByteAccess
    {
        byte[] ReadBytes(int offset, int length);
        string ReadAscii(int offset, int length);
        string ReadAsciiz(int offset);
        float  ReadFloat32(int offset);
        double ReadFloat64(int offset);
        sbyte  ReadInt8(int offset);
        short  ReadInt16(int offset);
        int    ReadInt32(int offset);
        long   ReadInt64(int offset);
        byte   ReadUInt8(int offset);
        ushort ReadUInt16(int offset);
        uint   ReadUInt32(int offset);
        ulong  ReadUInt64(int offset);

        void WriteBytes(int offset, byte[] toWrite);
        void WriteAscii(int offset, string toWrite);
        void WriteAsciiz(int offset, string toWrite);
        void WriteFloat32(int offset, float toWrite);
        void WriteFloat64(int offset, double toWrite);
        void WriteInt8(int offset, sbyte toWrite);
        void WriteInt16(int offset, short toWrite);
        void WriteInt32(int offset, int toWrite);
        void WriteInt64(int offset, long toWrite);
        void WriteUInt8(int offset, byte toWrite);
        void WriteUInt16(int offset, ushort toWrite);
        void WriteUInt32(int offset, uint toWrite);
        void WriteUInt64(int offset, ulong toWrite);
    }
}
