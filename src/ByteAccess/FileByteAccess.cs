using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace Quickbeam.ByteAccess
{
    public class FileByteAccessBuilder : IByteAccessBuilder
    {
        private MemoryMappedFile _handle;

        public FileByteAccessBuilder(string filepath)
        {
            _handle = MemoryMappedFile.CreateFromFile(filepath, FileMode.Open);
        }

        public IByteAccess CreateByteAccess(int offset, int size)
        {
            return new FileByteAccess(_handle.CreateViewAccessor(offset, size));
        }
        //class FileByteAccessBuilder(object):
        //    def __init__(self, filepath):
        //        self.handle = MemoryMappedFile.CreateFromFile(filepath, FileMode.Open)
        //
        //    def __call__(self, offset, size):
        //        return FileByteAccess(self.handle.CreateViewAccessor(offset, size))
        //
        //    def close(self):
        //        self.handle.close()
        //
    }

    public class FileByteAccess : IByteAccess
    {
        private static readonly ASCIIEncoding _encoding = new ASCIIEncoding();
        private MemoryMappedViewAccessor _f;

        public FileByteAccess(MemoryMappedViewAccessor f)
        {
            _f = f;
        }

        public byte[] ReadBytes(int offset, int size)
        {
            var bytes = new byte[size];
            _f.ReadArray<byte>(0, bytes, offset, size);
            return bytes;
        }

        public string ReadAscii(int offset, int size)
        {
            return _encoding.GetString(this.ReadBytes(offset, size));
        }

        public string ReadAsciiz(int offset)
        {
            throw new NotImplementedException();
        }

        public float  ReadSingle(int offset) { return _f.ReadSingle(offset); }
        public double ReadDouble(int offset) { return _f.ReadDouble(offset); }
        public sbyte  ReadInt8(  int offset) { return _f.ReadSByte(offset);  }
        public short  ReadInt16( int offset) { return _f.ReadInt16(offset);  }
        public int    ReadInt32( int offset) { return _f.ReadInt32(offset);  }
        public long   ReadInt64( int offset) { return _f.ReadInt64(offset);  }
        public byte   ReadUInt8( int offset) { return _f.ReadByte(offset);   }
        public ushort ReadUInt16(int offset) { return _f.ReadUInt16(offset); }
        public uint   ReadUInt32(int offset) { return _f.ReadUInt32(offset); }
        public ulong  ReadUInt64(int offset) { return _f.ReadUInt64(offset); }

        public void WriteBytes(int offset, byte[] toWrite)
        {
            throw new NotImplementedException();
        }

        public void WriteAscii(int offset, string toWrite)
        {
            throw new NotImplementedException();
        }

        public void WriteAsciiz(int offset, string toWrite)
        {
            throw new NotImplementedException();
        }

        public void WriteFloat32(int offset, float toWrite)  { _f.Write(offset, toWrite); }
        public void WriteFloat64(int offset, double toWrite) { _f.Write(offset, toWrite); }
        public void WriteInt8(   int offset, sbyte toWrite)  { _f.Write(offset, toWrite); }
        public void WriteInt16(  int offset, short toWrite)  { _f.Write(offset, toWrite); }
        public void WriteInt32(  int offset, int toWrite)    { _f.Write(offset, toWrite); }
        public void WriteInt64(  int offset, long toWrite)   { _f.Write(offset, toWrite); }
        public void WriteUInt8(   int offset, byte toWrite)   { _f.Write(offset, toWrite); }
        public void WriteUInt16(  int offset, ushort toWrite) { _f.Write(offset, toWrite); }
        public void WriteUInt32(  int offset, uint toWrite)   { _f.Write(offset, toWrite); }
        public void WriteUInt64(  int offset, ulong toWrite)  { _f.Write(offset, toWrite); }
    }
}
