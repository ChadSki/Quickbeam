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

import clr
import clrtype
from notifyproperty import notify_property, PyNotifyPropertyChanged

class ObservableField(PyNotifyPropertyChanged):
    """Subclasses are required to implement the notify_property 'Value'.
    """
    def __init__(self, parent):
        self.parent = parent

    def get_Value(self): return self.Value
    def set_Value(self, value): self.Value = value



################################################################
# floating point

def Float32Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'Float32FieldTemplate'
        @notify_property('Value')
        def Value(self): return float(self.parent.bytearray.ReadFloat32(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteFloat32(offset, value)
    return Field

def Float64Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'Float64FieldTemplate'
        @notify_property('Value')
        def Value(self): return float(self.parent.bytearray.ReadFloat64(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteFloat64(offset, value)
    return Field

################################################################
# integer

def Int8Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'Int8FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadInt8(parent.offset + offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteInt8(parent.offset + offset, value)
    return Field

def Int16Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'Int16FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadInt16(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteInt16(offset, value)
    return Field

def Int32Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'Int32FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadInt32(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteInt32(offset, value)
    return Field

def Int64Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'Int64FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadInt64(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteInt64(offset, value)
    return Field

def UInt8Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'UInt8FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadUInt8(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteUInt8(offset, value)
    return Field

def UInt16Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'UInt16FieldTemplate'
        @notify_property('Value')
        def Value(self): return self.parent.bytearray.ReadUInt16(offset)
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteUInt16(offset, value)
    return Field

def UInt32Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'UInt32FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadUInt32(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteUInt32(offset, value)
    return Field

def UInt64Field(offset):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'UInt64FieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadUInt64(offset))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteUInt64(offset, value)
    return Field

################################################################
# string

def RawDataField(offset, length):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'RawDataFieldTemplate'
        @notify_property('Value')
        def Value(self): return int(self.parent.bytearray.ReadBytes(offset, length))
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteBytes(offset, value)
    return Field

def AsciiField(offset, length, reverse):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'AsciiFieldTemplate'
        @notify_property('Value')
        def Value(self):
            answer = self.parent.bytearray.ReadAscii(offset, length)
            if reverse: answer = answer[::-1]
            return answer
        @Value.setter
        def Value(self, value):
            if reverse: value = value[::-1]
            self.parent.bytearray.WriteAscii(offset, value)
    return Field

def AsciizField(offset, maxlength):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'AsciizFieldTemplate'
        @notify_property('Value')
        def Value(self): return self.parent.bytearray.ReadAsciiz(offset, maxlength)
        @Value.setter
        def Value(self, value): self.parent.bytearray.WriteAsciiz(offset, value, maxlength)
    return Field

################################################################
# other

def ReferenceField(offset, loneid=False):
    if not loneid:
        offset += 12 # since we only care to read the ident, 12 bytes into a reference

    class Field(ObservableField):
        def get_TemplateKey(self): return 'ReferenceFieldTemplate'
        @notify_property('Value')
        def Value(self):
            ident = self.parent.bytearray.ReadUInt32(offset)
            if ident == 0 or ident == 0xFFFFFFFF:
                return None
            else:
                return None#self.parent.halomap.tags[ident] # return the referenced tag

        @Value.setter
        def Value(self, value):
            # when value is None, write Halo's version of null (-1)
            # otherwise, write the tag's ident, not the tag itself
            self.parent.bytearray.WriteUInt32(offset, 0xFFFFFFFF if value == None else value.ident)

    return Field


def ReflexiveField(offset, struct_class):
    class Field(ObservableField):
        def get_TemplateKey(self): return 'ReflexiveFieldTemplate'
        @notify_property('Value')
        def Value(self):
            count = self.parent.bytearray.ReadUInt32(offset)
            raw_offset = self.parent.bytearray.ReadUInt32(offset + 4)
            start_offset = raw_offset - self.map_magic

            return [struct_class(
                        self.parent.halomap.bytearraybuilder.new_bytearray(
                            start_offset + i * struct_class.size,
                            struct_class.size),
                        self.parent.halomap) for i in range(count)]
    return Field
