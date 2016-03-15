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

    The getf/setf functions translate between Python values and raw data from
    `byteaccess`."""

    def __init__(self, offset: int, docs=""):
        self.offset = offset
        self.docs = docs
        self.parent = None  # to be set by parent struct

    @abc.abstractmethod
    def getf(self):
        """To define a new type of field, subclass Field and override
        this getter function."""
        pass

    @abc.abstractmethod
    def setf(self, value):
        """To define a new type of field, subclass Field and override
        this setter function."""
        pass


################################################################
# Text fields

class Ascii(BasicField):

    """Fixed-length ascii string."""

    def __init__(self, *, offset, length, reverse=False, docs=""):
        super().__init__(offset, docs)
        self.length = length
        self.reverse = reverse

    def getf(self):
        buf = self.parent.byteaccess.read_ascii(self.offset, self.length)
        return buf[::-1] if self.reverse else buf

    def setf(self, newvalue):
        self.parent.byteaccess.write_ascii(
            self.offset, self.length, newvalue[::-1] if self.reverse else newvalue)


class Asciiz(BasicField):

    """Null-terminated ascii string."""

    def __init__(self, *, offset, maxlength, docs=""):
        super().__init__(offset, docs)
        self.maxlength = maxlength

    def getf(self):
        return self.parent.byteaccess.read_asciiz(self.offset, self.maxlength)

    def setf(self, newvalue):
        self.parent.byteaccess.write_asciiz(self.offset, self.maxlength, newvalue)


class RawData(BasicField):

    """Just bytes. Useful for debugging."""

    def __init__(self, *, offset, length, docs=""):
        super().__init__(offset, docs)
        self.length = length

    def getf(self):
        return self.parent.byteaccess.read_bytes(self.offset, self.length)

    def setf(self, newvalue):
        self.parent.byteaccess.WriteBytes(self.offset, newvalue)


################################################################
# Fields with options

class Enum16(BasicField):

    """16-bit enumeration of options."""

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
        value = self.parent.byteaccess.read_uint16(self.offset)
        try:
            return self.reverse_options[value]  # sometimes accessing throws

        except KeyError:
            print("enum16: Cannot find {} in options".format(value))
            return '???'  # Swallow exception, return placeholder

    def fset(self, newvalue):
        self.parent.byteaccess.write_uint16(self.offset, self.options[newvalue])


class Flag(BasicField):

    """A boolean flag."""

    def __init__(self, *, offset, bit, docs=""):
        super().__init__(offset, docs)
        self.bit = bit

    def fget(self):
        return self.parent.byteaccess.read_bit(self.offset, self.bit)

    def fset(self, newvalue):
        self.parent.byteaccess.write_bit(self.offset, self.bit, newvalue)


################################################################
# Number fields


class Float32(BasicField):

    """Floating point single-precision number."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_float32(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_float32(self.offset, newvalue),


class Float64(BasicField):

    """Floating point double-precision number."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_float64(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_float64(self.offset, newvalue),


class Int8(BasicField):

    """8-bit (1-byte) signed integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_int8(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_int8(self.offset, newvalue)


class Int16(BasicField):

    """16-bit (2-byte) signed integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_int16(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_int16(self.offset, newvalue)


class Int32(BasicField):

    """32-bit (4-byte) signed integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_int32(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_int32(self.offset, newvalue)


class Int64(BasicField):

    """64-bit (8-byte) signed integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_int64(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_int64(self.offset, newvalue)


class UInt8(BasicField):

    """8-bit (1-byte) unsigned integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_uint8(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_uint8(self.offset, self.value)


class UInt16(BasicField):

    """16-bit (2-byte) unsigned integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_uint16(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_uint16(self.offset, self.value)


class UInt32(BasicField):

    """32-bit (4-byte) unsigned integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_uint32(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_uint32(self.offset, self.value)


class UInt64(BasicField):

    """64-bit (8-byte) unsigned integer."""

    def __init__(self, *, offset, docs=""):
        super().__init__(offset, docs)

    def getf(self):
        return self.parent.byteaccess.read_uint64(self.offset)

    def setf(self, newvalue):
        self.parent.byteaccess.write_uint64(self.offset, self.value)
