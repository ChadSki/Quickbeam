# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import abc
from struct import pack, unpack


class BaseByteAccess(metaclass=abc.ABCMeta):

    """Abstract base class implements functionality common to all ByteAccesses.
    
    Provides generic methods for reading and writing different values. Subclasses
    need onlt implement _read_bytes and _write_bytes, and optionally a custom __init__."""

    def __init__(self, offset, size):
        """Provide access to a region of bytes.

        offset -- Absolute offset within the source medium.
        size -- Number of bytes to grant access to.
        """
        if offset < 0:
            raise ValueError("Offset must be 0 or larger. offset:{}"
                             .format(offset))
        if size <= 0:
            raise ValueError("Cannot create a ByteAccess with no size")
        self.offset = offset
        self.size = size

    @abc.abstractmethod
    def _read_bytes(self, offset, size):
        pass

    @abc.abstractmethod
    def _write_bytes(self, offset, to_write):
        pass

    # read/write bytestrings

    def read_bytes(self, offset, size):
        """Read a number of bytes from the source.

        offset -- Relative offset within the ByteAccess.
        size -- Number of bytes to read.
        """
        if offset < 0:
            raise ValueError("Offset must be positive. " +
                             "{} is invalid.".format(offset))

        if offset + size > self.size:
            raise ValueError("Cannot read past end of ByteAccess. " +
                             "offset:{0} size:{1} self.size:{2}"
                             .format(offset, size, self.size))

        return self._read_bytes(offset, size)

    def read_all_bytes(self):
        """Read all data this ByteAccess encapsulates."""
        return self.read_bytes(0, self.size)

    def write_bytes(self, offset, to_write):
        """Write a bytestring to the source.

        offset -- Relative offset within the ByteAccess.
        to_write -- The bytestring to write. If the bytestring is too long to
                    write at the specified offset, raises ValueError.
        """
        if offset < 0:
            raise ValueError("Offset must be positive. " +
                             "{} is invalid.".format(offset))

        if offset + len(to_write) > self.size:
            raise ValueError("Cannot write past end of ByteAccess. " +
                             "offset:{0} size:{1} self.size:{2}"
                             .format(offset, len(to_write), self.size))

        self._write_bytes(offset, to_write)

    # read/write bits

    def read_bit(self, offset, bit):
        if not 0 <= bit <= 7:
            raise ValueError("Bit must be 0-7.")
        byte = self.read_uint8(offset)
        return byte & (1 << bit) != 0

    def write_bit(self, offset, bit, data):
        if not 0 <= bit <= 7:
            raise ValueError("Bit must be 0-7.")

        byte = self.read_uint8(offset)
        if data:  # assign 1
            byte &= (1 << bit)
        else:  # assign 0
            byte |= ~(1 << bit)
        self.write_uint8(offset, byte)

    # read/write strings

    def read_ascii(self, offset, length):
        buf = self.read_bytes(offset, length)
        try:
            return buf.decode('ascii')
        except UnicodeDecodeError:
            return repr(buf)

    def write_ascii(self, offset, length, data):
        buf = data.encode('ascii')
        if len(buf) > length:
            raise ValueError("ascii string {} is {} characters too long"
                             .format(buf, len(buf) - length))
        elif len(buf) < length:
            buf += b'\x00' * (length - len(buf))
        self.write_bytes(offset, buf)

    def read_asciiz(self, offset, maxlength):
        buf = self.read_bytes(offset, maxlength)
        buf = buf[:buf.find(b'\x00')]  # truncate at null
        try:
            return buf.decode('ascii')
        except UnicodeDecodeError:
            return repr(buf)

    def write_asciiz(self, offset, maxlength, data):
        buf = (data + '\x00').encode('ascii')
        if len(buf) > maxlength:
            raise ValueError("ascii string {} is {} characters too long"
                             .format(buf, len(buf) - maxlength))
        self.write_bytes(offset, buf)

    def read_utf16(self, offset, maxlength):
        buf = self.read_bytes(offset, maxlength).decode('utf-16')
        if len(buf) > maxlength:
            raise ValueError("unicode string {} is {} characters too long"
                             .format(buf, len(buf) - maxlength))
        return buf[:buf.find('\x00')]  # null-terminated

    def write_utf16(self, offset, maxlength, data):
        buf = (data + '\0').encode('utf-16')
        self.write_bytes(offset, buf)

    # read/write numerics

    def read_float32(self, offset): return unpack('<f', self.read_bytes(offset, 4))[0]
    def read_float64(self, offset): return unpack('<d', self.read_bytes(offset, 8))[0]
    def read_int8(self, offset):    return unpack('<b', self.read_bytes(offset, 1))[0]
    def read_int16(self, offset):   return unpack('<h', self.read_bytes(offset, 2))[0]
    def read_int32(self, offset):   return unpack('<i', self.read_bytes(offset, 4))[0]
    def read_int64(self, offset):   return unpack('<q', self.read_bytes(offset, 8))[0]
    def read_uint8(self, offset):   return unpack('<B', self.read_bytes(offset, 1))[0]
    def read_uint16(self, offset):  return unpack('<H', self.read_bytes(offset, 2))[0]
    def read_uint32(self, offset):  return unpack('<I', self.read_bytes(offset, 4))[0]
    def read_uint64(self, offset):  return unpack('<Q', self.read_bytes(offset, 8))[0]
    def write_float32(self, offset, data): self.write_bytes(offset, pack('<f', data))
    def write_float64(self, offset, data): self.write_bytes(offset, pack('<d', data))
    def write_int8(self, offset, data):    self.write_bytes(offset, pack('<b', data))
    def write_int16(self, offset, data):   self.write_bytes(offset, pack('<h', data))
    def write_int32(self, offset, data):   self.write_bytes(offset, pack('<i', data))
    def write_int64(self, offset, data):   self.write_bytes(offset, pack('<q', data))
    def write_uint8(self, offset, data):   self.write_bytes(offset, pack('<B', data))
    def write_uint16(self, offset, data):  self.write_bytes(offset, pack('<H', data))
    def write_uint32(self, offset, data):  self.write_bytes(offset, pack('<I', data))
    def write_uint64(self, offset, data):  self.write_bytes(offset, pack('<Q', data))
