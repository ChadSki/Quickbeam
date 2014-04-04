using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Quickbeam.ByteAccess
{
    public abstract class BaseByteAccess
    {
        // Halo strings are ascii-only
        private static readonly ASCIIEncoding _encoding = new ASCIIEncoding();

        protected int _baOffset;
        protected int _baSize;

        // These are the only two methods that extensions of this abstract class
        // need to implement, other than a constructor
        protected abstract void writeBytes(int offset, byte[] data);
        protected abstract byte[] readBytes(int offset, int size);

        protected BaseByteAccess(int offset, int size)
        {
            _baOffset = offset;
            _baSize = size;
        }

        /// <summary>
        /// Performs bounds-checking, then reads bytes from the mapfile
        /// </summary>
        /// <param name="offset">The offset in bytes to write to</param>
        /// <param name="size">The number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(int offset, int size)
        {
            if ((offset + size) > (_baOffset + _baSize))
                throw new ArgumentException("Not allowed to read outside of MapAccess range");
            return readBytes(offset, size);
        }

        public int ReadInt32(int offset)
        {
            byte[] data = readBytes(offset, 4);
            return BitConverter.ToInt32(data, 0);
        }

        public short ReadInt16(int offset)
        {
            byte[] data = readBytes(offset, 2);
            return BitConverter.ToInt16(data, 0);
        }

        public float ReadSingle(int offset)
        {
            byte[] data = readBytes(offset, 4);
            return BitConverter.ToSingle(data, 0);
        }

        public string ReadString(int offset)
        {
            // I'm not aware of any good way to read a null-terminated string
            // from another process' memory, so we have to guess at the length
            // and try again if we didn't find the null.

            byte[] strBytes;
            int maxToRead = _baSize - offset;
            int bytesToRead = Math.Min(maxToRead, 64); // A decent starting value?

            do
            {
                strBytes = readBytes(offset, bytesToRead);
                bytesToRead *= 2;
            } while (!strBytes.ContainsNull() && (bytesToRead < maxToRead));

            GCHandle pinnedStrBytes = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
            IntPtr addrOfName = pinnedStrBytes.AddrOfPinnedObject();
            string result = Marshal.PtrToStringAnsi(addrOfName);
            pinnedStrBytes.Free();

            return result;
        }

        /// <summary>
        /// Performs bounds-checking, then writes bytes to the mapfile
        /// </summary>
        /// <param name="offset">The offset in bytes to write to</param>
        /// <param name="data">The data to write</param>
        public void WriteBytes(int offset, byte[] data)
        {
            if ((offset + data.Length) > (_baOffset + _baSize))
                throw new ArgumentException("Not allowed to write outside of MapAccess range");

            writeBytes(offset, data);
        }

        /// <summary>
        /// Writes an int to the mapfile, handling endianness
        /// </summary>
        /// <param name="toWrite">The int to write</param>
        /// <param name="offset">The offset in bytes to write to</param>
        public void WriteInt32(int offset, int toWrite)
        {
            byte[] data = BitConverter.GetBytes(toWrite);
            WriteBytes(offset, data);
        }

        public void WriteInt16(int offset, short toWrite)
        {
            byte[] data = BitConverter.GetBytes(toWrite);
            WriteBytes(offset, data);
        }

        public void WriteSingle(int offset, float toWrite)
        {
            byte[] data = BitConverter.GetBytes(toWrite);
            WriteBytes(offset, data);
        }

        // TODO - Does this include null-termination?
        /// <summary>
        /// Writes a string, as ascii, to the specified location in the map.
        /// </summary>
        /// <param name="toWrite">The string to write</param>
        /// <param name="offset">The offset in bytes to write to</param>
        public void WriteString(int offset, string toWrite)
        {
            byte[] data = _encoding.GetBytes(toWrite);
            WriteBytes(offset, data);
        }
    }
}
