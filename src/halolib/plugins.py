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
import fnmatch
import os
import xml.etree.ElementTree as et

from observablefield import *

from System import String, Object
from Quickbeam.Low import ObservableDictionary

def observable_field_class(field_type, offset, length, reverse, maxlength, options, reflexive_class):
    """Return an object which reads/writes to a field of a plugin-defined struct.
    """
    # select and create the appropriate field type
    return {
        'int8':      lambda: Int8Field(      offset),
        'int16':     lambda: Int16Field(     offset),
        'int32':     lambda: Int32Field(     offset),
        'int64':     lambda: Int64Field(     offset),
        'uint8':     lambda: UInt8Field(     offset),
        'uint16':    lambda: UInt16Field(    offset),
        'uint32':    lambda: UInt32Field(    offset),
        'uint64':    lambda: UInt64Field(    offset),
        'float32':   lambda: Float32Field(   offset),
        'float64':   lambda: Float64Field(   offset),

        'colorbyte': lambda: ColorByteField( offset),
        'colorRGB':  lambda: ColorRgbField(  offset),
        'colorARGB': lambda: ColorArgbField( offset),

        'enum8':     lambda: Enum8Field(     offset, options),
        'enum16':    lambda: Enum16Field(    offset, options),
        'enum32':    lambda: Enum32Field(    offset, options),

        'bitmask8':  lambda: Bitmask8Field(  offset, options),
        'bitmask16': lambda: Bitmask16Field( offset, options),
        'bitmask32': lambda: Bitmask32Field( offset, options),

        'rawdata':   lambda: RawDataField(   offset, length),
        'ascii':     lambda: AsciiField(     offset, length, reverse),
        'asciiz':    lambda: AsciizField(    offset, maxlength),

        'loneID':    lambda: ReferenceField( offset, False),
        'reference': lambda: ReferenceField( offset, True),
        'reflexive': lambda: ReflexiveField( offset, reflexive_class)
    }[field_type]()

def struct_class_from_xml(layout):
    """Define a new class based on the given struct layout.
    """
    # Parse xml once ahead of time, rather than in the constructor
    field_classes = {
        node.attrib['name']: observable_field_class(
            field_type = node.tag,
            offset = int(node.attrib.get('offset'), 0),
            length = int(node.attrib.get('length', '0'), 0),
            reverse = node.attrib.get('reverse', 'false') == 'true',
            maxlength = int(node.attrib.get('maxlength', '0'), 0),
            options = {
                opt.attrib['value']: opt.attrib['name'] for opt in node.iter('option')
            },
            reflexive_class = struct_class_from_xml(node) if node.tag == 'reflexive' else None,
        ) for node in layout
    }

    class HaloStruct(object):
        """Wraps an ObservableDictionary, presenting its fields as native Python properties.
        """

        # static variable, so ByteAccess objects know how large to be
        struct_size = int(layout.attrib['struct_size'], 0)

        def __init__(self, bytearray, halomap):
            object.__setattr__(self, 'bytearray', bytearray)
            object.__setattr__(self, 'halomap', halomap)

            # instantiate backing dictionary
            object.__setattr__(self, 'ObservableDictionary', ObservableDictionary[String, Object]())

            # fill with field fields
            for name in field_classes:
                field = field_classes[name](self)
                object.__getattribute__(self, 'ObservableDictionary')[name] = field

        def OnChanged(self, name):
            """Returns the dictionary's OnChanged event.
            """
            return object.__getattribute__(self, 'ObservableDictionary')[name].OnChanged

        def __repr__(self):
            """Pseudo-JSON format.
            """
            answer = '{'
            for pair in self.__dict__:
                answer += '\n    %s: ' % pair.Key
                lines = str(pair.Value.Value).split('\n')
                answer += lines.pop(0)
                for line in lines:
                    answer += '\n    ' + line
                answer += ','
            answer += '\n}'
            return answer

        def __dir__(self):
            """Returns names of the ObservableDictionary's fields.
            """
            return object.__getattribute__(self, 'ObservableDictionary').Keys

        def __getattribute__(self, name):
            """Redirects lookup to the ObservableDictionary's fields.
            """
            if name == 'bytearray':
                return object.__getattribute__(self, 'bytearray')

            elif name == 'halomap':
                return object.__getattribute__(self, 'halomap')

            elif name == '__dict__':
                # so C# can get the ObservableDictionary for databinding
                return object.__getattribute__(self, 'ObservableDictionary')

            elif name == 'OnChanged':
                return object.__getattribute__(self, 'OnChanged')

            else:
                # pass lookup through to the ObservableDictionary
                return object.__getattribute__(self, 'ObservableDictionary')[name].Value

        def __setattr__(self, name, value):
            """Redirects lookup to the ObservableDictionary's fields.
            """
            object.__getattribute__(self, 'ObservableDictionary')[name].Value = value

    return HaloStruct

plugin_classes = {}

def load_plugins():
    """Generates struct-wrapping classes based on xml plugins.
    """
    src_dir = os.path.dirname(os.path.abspath(__file__))
    plugins_dir = os.path.join(src_dir, 'plugins')
    for dirpath, dirnames, files in os.walk(plugins_dir):
        for filename in fnmatch.filter(files, '*.xml'):
            root_struct = et.parse(os.path.join(plugins_dir, filename)).getroot()  # load the xml definition
            plugin_classes[root_struct.attrib['name']] = struct_class_from_xml(root_struct)
