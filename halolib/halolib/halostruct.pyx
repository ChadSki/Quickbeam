# cython: language_level=3

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

import glob
import os
import xml.etree.ElementTree as et
from halolib.cythonutil cimport *
from halolib.cythonutil import *

plugin_classes = {}

class HaloStruct(object):
    """A base class for Halo structs which contain primitive (int, float, string)
    and composite (reference, RGB color) fields. Additionally, a struct may
    contain 'reflexives', which involve pointers to other structs. Struct layouts
    are defined in XML.
    """
    def __init__(self, access, map_magic, halomap):
        self.access = access
        self.map_magic = map_magic
        self.halomap = halomap

    def __str__(self):
        def struct_to_s(struct, answer='', indent='\n    '):
            """Recursively stringifies the base struct as well as reflexives."""
            for field_name in struct.field_names:
                value = getattr(struct, field_name)

                if type(value) == list:
                    for i, reflexive_chunk in enumerate(value):
                        answer += indent + field_name + '[%d]:' % i
                        answer = struct_to_s(reflexive_chunk, answer, indent + '    ')
                else:
                    answer += indent + field_name + ': ' + str(value)

            return answer

        return struct_to_s(self) + '\n'


def make_field_property(field_type, reflexive_class=None, **kwargs):
    """Defines a property which reads/writes to a field of a plugin-defined HaloStruct."""

    offset = int(kwargs.get('offset'), base=0)

    if field_type == 'reflexive':
        @property
        def field(self):
            buf = self.access.read_bytes(offset, 8)
            count = read_uint32(<int><char*>buf)
            raw_offset = read_uint32(<int><char*>buf + 4)

            start_offset = raw_offset - self.map_magic

            # list of reflexive structs
            return [reflexive_class(
                        self.access.__class__(
                            start_offset + i * reflexive_class.struct_size,
                            reflexive_class.struct_size),
                        self.map_magic,
                        self.halomap) for i in range(count)]

        @field.setter
        def field(self, value):
            raise Exception("Not yet implemented")

    elif field_type == 'reference':
        offset += 12 # since we only care to read the ident, 12 bytes into a reference

        @property
        def field(self):
            buf = self.access.read_bytes(offset, 4)
            ident = read_uint32(<int><char*>buf)

            if ident == 0 or ident == 0xFFFFFFFF:
                return None
            else:
                return self.halomap.tags[ident] # return the referenced tag

        @field.setter
        def field(self, value):
            if value:
                ident = value.ident             # write back the tag's ident, not the tag itself
            else:
                ident = 0xFFFFFFFF              # Halo's version of null

            buf = self.access.read_bytes(offset, 4)
            write_uint32(<int><char*>buf, ident)
            self.access.write_bytes(buf, offset)

    # both of these are fixed-length bytestrings, but ascii is decoded
    # from bytes to a Python string
    elif field_type in ['rawdata', 'ascii']:
        length = int(kwargs.get('length', '0'), base=0)
        reverse = kwargs.get('reverse', 'false')

        @property
        def field(self):
            buf = self.access.read_bytes(offset, length)
            answer = b'\x00' * length
            memcpy(<char*>answer, <char*>buf, length)

            if field_type == 'ascii': answer = answer.decode('ascii')
            if reverse == 'true': answer = answer[::-1]
            return answer

        @field.setter
        def field(self, value):
            if reverse == 'true': value = value[::-1]
            if field_type == 'ascii': value = value.encode('ascii')

            buf = self.access.read_bytes(offset, length)
            memcpy(<char*>buf, <char*>value, length)
            self.access.write_bytes(buf, offset)

    # null-terminated strings are a bit different
    elif field_type == 'asciiz':
        maxlength = int(kwargs.get('maxlength', '0'), base=0)

        @property
        def field(self):
            buf = self.access.read_bytes(offset, maxlength)

            # stop at null-termination, but limit to a maximum
            length = min(strlen(<char*>buf), maxlength)

            answer = b'\x00' * length
            memcpy(<char*>answer, <char*>buf, length)
            return answer.decode('ascii')

        @field.setter
        def field(self, value):
            # stop at null-termination, but limit to a maximum
            # (leaving the last byte for a null character)
            length = min(strlen(<char*>value), maxlength - 1)

            buf = self.access.read_bytes(offset, length)
            memcpy(<char*>buf, <char*>value, length)

            null_loc = <int><char*>buf + length
            memcpy(<char*>null_loc, <char*>b'\x00', 1)

            self.access.write_bytes(buf, offset)

    else: # primitives
        read_fn, write_fn, size_of = {
            'float32': (read_float32, write_float32, 4),
            'float64': (read_float64, write_float64, 8),
            'int8':    (read_int8,    write_int8,    1),
            'int16':   (read_int16,   write_int16,   2),
            'int32':   (read_int32,   write_int32,   4),
            'int64':   (read_int64,   write_int64,   8),
            'uint8':   (read_uint8,   write_uint8,   1),
            'uint16':  (read_uint16,  write_uint16,  2),
            'uint32':  (read_uint32,  write_uint32,  4),
            'uint64':  (read_uint64,  write_uint64,  8),
        }[field_type]

        @property
        def field(self):
            buf = self.access.read_bytes(offset, size_of)
            return read_fn(<int><char*>buf)

        @field.setter
        def field(self, value):
            buf = self.access.read_bytes(offset, size_of)
            write_fn(<int><char*>buf, value)
            self.access.write_bytes(buf, offset)

    return field

def class_from_xml(layout):
    """Return a new class which wraps a c-struct. Extends HaloStruct by adding
    properties to read/write to fields. Field layouts are specified by xml plugins.
    """
    # define a subclass of HaloStruct using the name from xml
    new_class = type(layout.attrib['name'], (HaloStruct,), {})

    new_class.struct_size = int(layout.attrib['struct_size'], base=0)

    new_class.field_names = []
    new_class.reflexive_names = []

    for node in layout:
        field_type = node.tag
        field_name = node.attrib['name']
        field_options = node.attrib

        new_class.field_names.append(field_name)

        reflexive_class = None
        if field_type == 'reflexive':
            new_class.reflexive_names.append(field_name)
            reflexive_class = class_from_xml(node)

        setattr(new_class, field_name,
            make_field_property(field_type, reflexive_class, **field_options))

    return new_class

def load_plugins():
    """Load all xml plugins from the plugin directory."""
    plugin_list = glob.glob(os.path.join('.\plugins', '*.xml'))
    for filepath in plugin_list:
        root_struct = et.parse(filepath).getroot()  # load the xml definition
        plugin_classes[root_struct.attrib['name']] = class_from_xml(root_struct)
