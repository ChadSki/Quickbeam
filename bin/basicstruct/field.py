# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import abc


class BasicField(metaclass=abc.ABCMeta):

    """Represents a single field within a struct, and brokers read/write access
    to the underlying bits wherever they may reside.

    Fields are parented by a struct, which can be accessed via `self.parent`.

    The getf/setf functions do not cache data, but convert to and from raw data via
    `byteaccess`."""

    typestring = 'undefined'
    """Declares this field's type in a way easy to access by other code relying on
    this library."""

    def __init__(self, offset: int, docs=""):
        self.offset = offset
        self.docs = docs
        self.parent = None  # to be set by parent struct

    @abc.abstractmethod
    def getf(self, byteaccess):
        """To define a new type of field, subclass Field and override
        this getter function."""
        pass

    @abc.abstractmethod
    def setf(self, byteaccess, value):
        """To define a new type of field, subclass Field and override
        this setter function."""
        pass


################################################################
# Text fields

class Ascii(BasicField):

    """Fixed-length ascii string."""

    typestring = 'ascii'

    def __init__(self, *, offset, length, reverse=False, docs=""):
        super().__init__(offset, docs)
        self.length = length
        self.reverse = reverse

    def getf(self, byteaccess):
        buf = byteaccess.read_ascii(self.offset, self.length)
        return buf[::-1] if self.reverse else buf

    def setf(self, byteaccess, newvalue):
        self.write_ascii(self.offset, self.length,
                         newvalue[::-1] if self.reverse else newvalue)


class Asciiz(BasicField):

    """Null-terminated ascii string."""

    typestring = 'asciiz'

    def __init__(self, *, offset, maxlength, docs=""):
        super().__init__(offset, docs)
        self.maxlength = maxlength

    def getf(self, byteaccess):
        return byteaccess.read_asciiz(self.offset, self.maxlength)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_asciiz(self.offset, self.maxlength, newvalue)


class RawData(BasicField):

    """Just bytes. Useful for debugging."""

    typestring = 'rawdata'

    def __init__(self, *, offset, length, docs=""):
        super().__init__(offset, docs)
        self.length = length

    def getf(self, byteaccess):
        return byteaccess.read_bytes(self.offset, self.length)

    def setf(self, byteaccess, newvalue):
        byteaccess.WriteBytes(self.offset, newvalue)


################################################################
# Fields with options

class Enum16(BasicField):

    """16-bit enumeration of options."""

    typestring = 'enum16'

    def __init__(self, *, offset, options, docs=""):
        super().__init__(offset, docs)

        self.options = options
        # type: Dict[str, int]

        self.reverse_options = {
            int(number): name for name, number in options.items()
        }  # type: Dict[int, str]

        self.docstring = '\n'.join('    {} => {}'.format(number, name)
                                   for name, number in options.items())

        raise NotImplementedError()

    def fget(self):
        value = byteaccess.read_uint16(self.offset)
        try:
            return self.reverse_options[value]  # sometimes accessing throws

        except KeyError:
            print("enum16: Cannot find {} in options".format(value))
            return '???'  # Swallow exception, return placeholder

    def fset(self, newvalue):
        byteaccess.write_uint16(self.offset, self.options[newvalue])


class Flag(BasicField):

    """A boolean flag."""

    typestring = 'flag'

    def __init__(self, *, offset, bit, docs=""):
        super().__init__(offset, docs)
        self.bit = bit

    def fget(self):
        return byteaccess.read_bit(self.offset, self.bit)

    def fset(self, newvalue):
        byteaccess.write_bit(self.offset, self.bit, newvalue)


################################################################
# Number fields


class Float32(BasicField):

    """Floating point single-precision number."""

    typestring = 'float32'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_float32(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_float32(self.offset, newvalue),


class Float64(BasicField):

    """Floating point double-precision number."""

    typestring = 'float64'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_float64(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_float64(self.offset, newvalue),


class Int8(BasicField):

    """8-bit (1-byte) signed integer."""

    typestring = 'int8'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_int8(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_int8(self.offset, newvalue)


class Int16(BasicField):

    """16-bit (2-byte) signed integer."""

    typestring = 'int16'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_int16(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_int16(self.offset, newvalue)


class Int32(BasicField):

    """32-bit (4-byte) signed integer."""

    typestring = 'int32'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_int32(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_int32(self.offset, newvalue)


class Int64(BasicField):

    """64-bit (8-byte) signed integer."""

    typestring = 'int64'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_int64(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_int64(self.offset, newvalue)


class UInt8(BasicField):

    """8-bit (1-byte) unsigned integer."""

    typestring = 'uint8'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_uint8(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_uint8(self.offset, self.value)


class UInt16(BasicField):

    """16-bit (2-byte) unsigned integer."""

    typestring = 'uint16'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_uint16(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_uint16(self.offset, self.value)


class UInt32(BasicField):

    """32-bit (4-byte) unsigned integer."""

    typestring = 'uint32'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_uint32(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_uint32(self.offset, self.value)


class UInt64(BasicField):

    """64-bit (8-byte) unsigned integer."""

    typestring = 'uint64'

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self, byteaccess):
        return byteaccess.read_uint64(self.offset)

    def setf(self, byteaccess, newvalue):
        byteaccess.write_uint64(self.offset, self.value)
