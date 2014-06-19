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
import clr
import fnmatch
import os
import xml.etree.ElementTree as et

def make_observable_field(bytearray, halomap, field_type, offset, length, reverse, maxlength, options, reflexive_class):
    """Return an object which reads/writes to a field of a plugin-defined struct.
    """
    # select and create the appropriate field type
    return {
        'int8':      lambda: Int8Field(      bytearray, offset),
        'int16':     lambda: Int16Field(     bytearray, offset),
        'int32':     lambda: Int32Field(     bytearray, offset),
        'int64':     lambda: Int64Field(     bytearray, offset),
        'uint8':     lambda: UInt8Field(     bytearray, offset),
        'uint16':    lambda: UInt16Field(    bytearray, offset),
        'uint32':    lambda: UInt32Field(    bytearray, offset),
        'uint64':    lambda: UInt64Field(    bytearray, offset),
        'float32':   lambda: Float32Field(   bytearray, offset),
        'float64':   lambda: Float64Field(   bytearray, offset),

        'colorbyte': lambda: ColorByteField( bytearray, offset),
        'colorRGB':  lambda: ColorRgbField(  bytearray, offset),
        'colorARGB': lambda: ColorArgbField( bytearray, offset),

        'enum8':     lambda: Enum8Field(     bytearray, offset, options),
        'enum16':    lambda: Enum16Field(    bytearray, offset, options),
        'enum32':    lambda: Enum32Field(    bytearray, offset, options),

        'bitmask8':  lambda: Bitmask8Field(  bytearray, offset, options),
        'bitmask16': lambda: Bitmask16Field( bytearray, offset, options),
        'bitmask32': lambda: Bitmask32Field( bytearray, offset, options),

        'rawdata':   lambda: RawDataField(   bytearray, offset, length, reverse),
        'ascii':     lambda: AsciiField(     bytearray, offset, length, reverse),
        'asciiz':    lambda: AsciizField(    bytearray, offset, maxlength),

        'loneID':    lambda: ReferenceField( bytearray, offset, halomap, False),
        'reference': lambda: ReferenceField( bytearray, offset, halomap, True),
        'reflexive': lambda: ReflexiveField( bytearray, offset, halomap, reflexive_class)
    }[field_type]()

def class_from_xml(layout):
    """Define a new class based on the given struct layout.
    """
    # Parse xml once ahead of time, rather than in the constructor
    # { field name => make_observable_field params }
    field_params = {
        node.attrib['name']: {
            'field_type': node.tag,
            'offset': int(node.attrib.get('offset'), 0),
            'length': int(node.attrib.get('length', '0'), 0),
            'reverse': node.attrib.get('reverse', 'false') == 'true',
            'maxlength': int(node.attrib.get('maxlength', '0'), 0),
            'options': {
                opt.attrib['value']: opt.attrib['name'] for opt in node.iter('option')
            },
            'reflexive_class': class_from_xml(node) if node.tag == 'reflexive' else None,
        } for node in layout
    }

    class HaloStruct(object):
        """Wraps an ObservableStruct, presenting its fields as native Python properties.
        """

        # static variable, so ByteAccess objects know how large to be
        struct_size = int(layout.attrib['struct_size'], 0)

        def __init__(self, byteaccess, halomap):
            # instantiate backing ObservableStruct
            object.__setattr__(self, 'ObservableStruct', ObservableStruct())

            # fill with field fields
            for name in field_params:
                field = make_observable_field(byteaccess, halomap, **field_params[name])
                object.__getattribute__(self, 'ObservableStruct')[name] = field

        def OnChanged(self, name):
            """Returns the ObservableStruct's OnChanged event.
            """
            return object.__getattribute__(self, 'ObservableStruct')[name].OnChanged

        def __repr__(self):
            answer = '{'
            for pair in self.__dict__:
                answer += '\n    "%s": '
                lines = str(pair.Value).split('\n')
                answer += lines.pop()
                for line in lines:
                    answer += '\n    ' + line
                answer += ','
            answer += '\n}'
            return answer

        def __dir__(self):
            """Returns names of the ObservableStruct's fields.
            """
            return object.__getattribute__(self, 'ObservableStruct').Keys

        def __getattribute__(self, name):
            """Redirects lookup to the ObservableStruct's fields.
            """
            if name == '__dict__':
                # so C# can get the backing ObservableStruct for databinding
                return object.__getattribute__(self, 'ObservableStruct')

            elif name == 'OnChanged':
                return object.__getattribute__(self, 'OnChanged')

            else:
                # pass lookup through to the ObservableStruct
                return object.__getattribute__(self, 'ObservableStruct')[name].Value

        def __setattr__(self, name, value):
            """Redirects lookup to the ObservableStruct's fields.
            """
            object.__getattribute__(self, 'ObservableStruct')[name].Value = value

    return HaloStruct

plugin_classes = {}

def load_plugins():
    """Generates struct-wrapping classes based on xml plugins.
    """
    src_dir = os.path.dirname(os.path.abspath(__file__))
    plugins_dir = os.path.join(src_dir, 'plugins')
    for dirpath, dirnames, files in os.walk(plugins_dir):
        for filename in fnmatch.filter(files, '*.xml'):
            root_struct = et.parse(filename).getroot()  # load the xml definition
            plugin_classes[root_struct.attrib['name']] = class_from_xml(root_struct)
