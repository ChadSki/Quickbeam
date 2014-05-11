# Copyright (c) 2013, Chad Zawistowski
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are met:
#     * Redistributions of source code must retain the above copyright
#       notice, this list of conditions and the following disclaimer.
#     * Redistributions in binary form must reproduce the above copyright
#       notice, this list of conditions and the following disclaimer in the
#       documentation and/or other materials provided with the distribution.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
# DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
# (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
# ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
# SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

from __future__ import print_function
import pyevent

from notifyproperty import PyNotifyPropertyChanged, notify_property

class ObservableField(PyNotifyPropertyChanged):
    def __init__(self, byteaccess, offset):
        self.access = byteaccess
        self.offset = offset

    def __str__(self):
        return str(self.Value)


# floating-point

class Float32Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadFloat32(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteFloat32(self.offset, value)

class Float64Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadFloat64(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteFloat16(self.offset, value)

# integer

class Int8Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadInt8(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteInt8(self.offset, value)

class Int16Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadInt16(offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteInt16(offset, value)

class Int32Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadInt32(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteInt32(self.offset, value)

class Int64Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadInt64(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteInt16(self.offset, value)

class UInt8Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadUInt8(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteUInt8(self.offset, value)

class UInt16Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadUInt16(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteUInt16(self.offset, value)

class UInt32Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadUInt32(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteUInt32(self.offset, value)

class UInt64Field(ObservableField):
    @notify_property('Value')
    def Value(self):
        return self.access.ReadUInt64(self.offset)
    @Value.setter
    def Value(self, value):
        self.access.WriteUInt16(self.offset, value)

# bitmask

def Bitmask8Field(ObservableField):
    @notify_property('Value')
    def Value(self): return None
    @Value.setter
    def Value(self, value):
        toWrite = sum(1<<i for i, flag in enumerate(value) if flag)
        self.access.WriteUInt8(toWrite)

def Bitmask16Field(ObservableField):
    @notify_property('Value')
    def Value(self): return None
    @Value.setter
    def Value(self, value):
        toWrite = sum(1<<i for i, flag in enumerate(value) if flag)
        self.access.WriteUInt16(toWrite)

def Bitmask32Field(ObservableField):
    @notify_property('Value')
    def Value(self): return None
    @Value.setter
    def Value(self, value):
        toWrite = sum(1<<i for i, flag in enumerate(value) if flag)
        self.access.WriteUInt32(toWrite)

# enum

def Enum8Field(UInt8Field):
    def __init__(self, byteaccess, offset, options):
        self.access = byteaccess
        self.offset = offset
        self.options = options

def Enum16Field(UInt8Field):
    def __init__(self, byteaccess, offset, options):
        self.access = byteaccess
        self.offset = offset
        self.options = options

def Enum32Field(UInt8Field):
    def __init__(self, byteaccess, offset, options):
        self.access = byteaccess
        self.offset = offset
        self.options = options

# reference

def ReferenceField(ObservableField):
    def __init__(self, byteaccess, offset, halomap):
        self.access = byteaccess
        self.offset = offset
        self.halomap = halomap

    @notify_property('Value')
    def Value(self):
        return self.halomap.tags[self.access.ReadUInt32(self.offset)]

# reflexive

def ReflexiveField(ObservableField):
    def __init__(self, byteaccess, offset, halomap, reflexive_class):
        self.access = byteaccess
        self.offset = offset
        self.halomap = halomap
        self.reflexive_class

    @notify_property('Value')
    def Value(self):
        count = self.access.ReadUInt32(0);
        rawOffset = self.access.ReadUInt32(4);
        startOffset = rawOffset - self.halomap.map_magic;
        for i in range(count):
            yield ReflexiveClass(
                    self.halomap.ByteAccess(
                        startOffset + ReflexiveClass.struct_size * i,
                        ReflexiveClass.struct_size),
                    HaloMap)

    @Value.setter
    def Value(self, value):
        raise Exception()


x = Int8Field(None, 0)
print(x.GetType().ToString())
print(x.__class__.__name__)

