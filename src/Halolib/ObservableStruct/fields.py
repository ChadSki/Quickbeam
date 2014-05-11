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

from notifyproperty import NotifyProperty

################################################################
# floating point

def float32(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_float32(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_float32(self.offset + offset, value))

def float64(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_float64(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_float64(self.offset + offset, value))

################################################################
# integer

def int8(offset):
    return lambda parent: NotifyProperty(
        fget=lambda self: parent.bytearray.read_int8(parent.offset + offset),
        fset=lambda self, value: parent.bytearray.write_int8(parent.offset + offset, value))

def int16(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_int16(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_int16(self.offset + offset, value))

def int32(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_int32(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_int32(self.offset + offset, value))

def int64(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_int64(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_int64(self.offset + offset, value))

def uint8(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_uint8(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_uint8(self.offset + offset, value))

def uint16(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_uint16(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_uint16(self.offset + offset, value))

def uint32(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_uint32(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_uint32(self.offset + offset, value))

def uint64(offset):
    return NotifyProperty(
        fget=lambda self: self.bytearray.read_uint64(self.offset + offset),
        fset=lambda self, value: self.bytearray.write_uint64(self.offset + offset, value))

################################################################

def reference(offset, struct_class, loneid=False):
    if not loneid:
        offset += 12 # since we only care to read the ident, 12 bytes into a reference

    @NotifyProperty
    def field(self):
        ident = self.bytearray.read_uint32(self.offset + offset)
        if ident == 0 or ident == 0xFFFFFFFF:
            return None
        else:
            return self.halomap.tags[ident] # return the referenced tag

    @field.setter
    def field(self, value):
        # when value is None, write Halo's version of null (-1)
        # otherwise, write the tag's ident, not the tag itself
        self.bytearray.write_uint32(offset, 0xFFFFFFFF if value == None else value.ident)

    return field


def reflexive(offset, struct_class):
    @NotifyProperty
    def field(self):
        count = self.bytearray.read_uint32(self.offset + offset)
        raw_offset = self.bytearray.read_uint32(self.offset + offset + 4)
        start_offset = raw_offset - self.map_magic

        return [struct_class(
                    self.halomap.bytearraybuilder.new_bytearray(
                        start_offset + i * struct_class.size,
                        struct_class.size),
                    self.halomap) for i in range(count)]
    return field
