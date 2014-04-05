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
