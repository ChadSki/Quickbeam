using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Quickbeam.Low.ByteAccess
{
    /// <summary>
    /// Provides a default implementation for most of IByteAccess.
    /// </summary>
    /// <remarks>
    /// Override ReadBytesImpl and WriteBytesImpl to complete your class.
    /// </remarks>
    public abstract class BaseByteAccess : IByteAccess
    {
        // Halo strings are ascii-only
        protected static readonly ASCIIEncoding Encoding = new ASCIIEncoding();

        protected int Offset;
        protected int Size;

        protected BaseByteAccess(int offset, int size)
        {
            Offset = offset;
            Size = size;
        }


        #region bytearrays
        /// <summary>
        /// Performs bounds-checking, then reads bytes from the mapfile
        /// </summary>
        public byte[] ReadBytes(int offset, int length)
        {
            if ((offset + length) > (Offset + Size))
                throw new ArgumentException("offset + length too large: not allowed to read outside of MapAccess range");
            return ReadBytesImpl(offset, length);
        }

        protected abstract byte[] ReadBytesImpl(int offset, int length);

        /// <summary>
        /// Performs bounds-checking, then writes bytes to the mapfile
        /// </summary>
        public void WriteBytes(int offset, byte[] data)
        {
            if ((offset + data.Length) > (Offset + Size))
                throw new ArgumentException("Not allowed to write outside of MapAccess range");

            WriteBytesImpl(offset, data);
        }

        protected abstract void WriteBytesImpl(int offset, byte[] data);
        #endregion

        #region string types
        public string ReadAscii(int offset, int length)
        {
            var strBytes = ReadBytes(offset, length);
            var pinnedStrBytes = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
            var addrOfName = pinnedStrBytes.AddrOfPinnedObject();
            var result = Marshal.PtrToStringAnsi(addrOfName);
            pinnedStrBytes.Free();
            return result;
        }

        public string ReadAsciiz(int offset)
        {
            // I'm not aware of any good way to read a null-terminated string
            // from another process' memory, so we have to guess at the length
            // and try again if we didn't find the null.

            byte[] strBytes;
            int maxToRead = Size - offset;
            int bytesToRead = Math.Min(/* A decent starting value? */ 64, maxToRead); //TODO profiling for a good starting value

            do
            {
                strBytes = ReadBytesImpl(offset, bytesToRead);
                bytesToRead *= 2;
            } while (!strBytes.Any(b => b == (byte)0x00) && (bytesToRead < maxToRead));

            var pinnedStrBytes = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
            var addrOfName = pinnedStrBytes.AddrOfPinnedObject();
            var result = Marshal.PtrToStringAnsi(addrOfName);
            pinnedStrBytes.Free();
            return result;
        }

        /// <summary>
        /// Writes a string, as ascii, to the specified location in the map.
        /// </summary>
        /// <param name="toWrite">The string to write</param>
        /// <param name="offset">The offset in bytes to write to</param>
        // TODO - Does this include null-termination? It should not.
        public void WriteAscii(int offset, string toWrite) { WriteBytes(offset, Encoding.GetBytes(toWrite)); }

        /// <summary>
        /// Writes a string, as ascii, to the specified location in the map.
        /// </summary>
        /// <param name="toWrite">The string to write</param>
        /// <param name="offset">The offset in bytes to write to</param>
        // TODO - Does this include null-termination? It should.
        public void WriteAsciiz(int offset, string toWrite) { WriteBytes(offset, Encoding.GetBytes(toWrite)); }
        #endregion

        #region integer types
        public sbyte ReadInt8(int offset) { return (sbyte)ReadBytesImpl(offset, 1).First(); }
        public short ReadInt16(int offset) { return BitConverter.ToInt16(ReadBytesImpl(offset, 2), 0); }
        public int ReadInt32(int offset) { return BitConverter.ToInt32(ReadBytesImpl(offset, 4), 0); }
        public long ReadInt64(int offset) { return BitConverter.ToInt64(ReadBytesImpl(offset, 8), 0); }
        public byte ReadUInt8(int offset) { return ReadBytesImpl(offset, 1).First(); }
        public ushort ReadUInt16(int offset) { return BitConverter.ToUInt16(ReadBytesImpl(offset, 2), 0); }
        public uint ReadUInt32(int offset) { return BitConverter.ToUInt32(ReadBytesImpl(offset, 4), 0); }
        public ulong ReadUInt64(int offset) { return BitConverter.ToUInt64(ReadBytesImpl(offset, 8), 0); }
        public void WriteInt8(int offset, sbyte toWrite) { WriteBytes(offset, new[] { (byte)toWrite }); }
        public void WriteInt16(int offset, short toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        public void WriteInt32(int offset, int toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        public void WriteInt64(int offset, long toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }

        public void WriteUInt8(int offset, byte toWrite) { WriteBytes(offset, new[] { toWrite }); }
        public void WriteUInt16(int offset, ushort toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        public void WriteUInt32(int offset, uint toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        public void WriteUInt64(int offset, ulong toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        #endregion

        #region float types
        public float ReadFloat32(int offset) { return BitConverter.ToSingle(ReadBytesImpl(offset, 4), 0); }
        public double ReadFloat64(int offset) { return BitConverter.ToDouble(ReadBytesImpl(offset, 8), 0); }
        public void WriteFloat32(int offset, float toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        public void WriteFloat64(int offset, double toWrite) { WriteBytes(offset, BitConverter.GetBytes(toWrite)); }
        #endregion
    }
}
