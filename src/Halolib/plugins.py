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
import glob
import xml.etree.ElementTree as et

def make_observable_field(byteaccess, halomap, field_type, offset, length, reverse, maxlength, options, reflexive_class):
    """Return an object which reads/writes to a field of a plugin-defined struct.
    """
    # select and create the appropriate field type
    return {
        'int8':      lambda: Int8Field(      byteaccess, offset),
        'int16':     lambda: Int16Field(     byteaccess, offset),
        'int32':     lambda: Int32Field(     byteaccess, offset),
        'int64':     lambda: Int64Field(     byteaccess, offset),
        'uint8':     lambda: UInt8Field(     byteaccess, offset),
        'uint16':    lambda: UInt16Field(    byteaccess, offset),
        'uint32':    lambda: UInt32Field(    byteaccess, offset),
        'uint64':    lambda: UInt64Field(    byteaccess, offset),
        'float32':   lambda: Float32Field(   byteaccess, offset),
        'float64':   lambda: Float64Field(   byteaccess, offset),

        'colorbyte': lambda: ColorByteField( byteaccess, offset),
        'colorRGB':  lambda: ColorRgbField(  byteaccess, offset),
        'colorARGB': lambda: ColorArgbField( byteaccess, offset),

        'enum8':     lambda: Enum8Field(     byteaccess, offset, options),
        'enum16':    lambda: Enum16Field(    byteaccess, offset, options),
        'enum32':    lambda: Enum32Field(    byteaccess, offset, options),

        'bitmask8':  lambda: Bitmask8Field(  byteaccess, offset, options),
        'bitmask16': lambda: Bitmask16Field( byteaccess, offset, options),
        'bitmask32': lambda: Bitmask32Field( byteaccess, offset, options),

        'rawdata':   lambda: RawDataField(   byteaccess, offset, length, reverse),
        'ascii':     lambda: AsciiField(     byteaccess, offset, length, reverse),
        'asciiz':    lambda: AsciizField(    byteaccess, offset, maxlength),

        'loneID':    lambda: ReferenceField( byteaccess, offset, halomap, False),
        'reference': lambda: ReferenceField( byteaccess, offset, halomap, True),
        'reflexive': lambda: ReflexiveField( byteaccess, offset, halomap, reflexive_class)
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
    for filepath in glob.glob('.\plugins\*.xml'):
        root_struct = et.parse(filepath).getroot()  # load the xml definition
        plugin_classes[root_struct.attrib['name']] = class_from_xml(root_struct)
