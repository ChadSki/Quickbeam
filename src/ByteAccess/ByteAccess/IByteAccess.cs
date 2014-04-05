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
