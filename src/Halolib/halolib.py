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
from plugins import load_plugins, plugin_classes
from halotag import HaloTag

from System import Array, Byte
from Quickbeam.Low.ByteArray import FileByteArrayBuilder
from Quickbeam.Low.ByteArray import WinMemoryByteArrayBuilder

# ensure plugins are loaded
if len(plugin_classes) == 0:
    load_plugins()

class HaloMap(object):
    def __init__(self):
        self.bytearraybuilder = None
        self.magic = None

def load_map(map_path=None):
    """Loads a map from Halo.exe's memory, or from disk if given a filepath. Loading a
    map requires that plugins have already been loaded.
    """
    # end result, assembled piece-by-piece
    halomap = HaloMap()

    if map_path != None:
        location = 'file'
        halomap.bytearraybuilder = FileByteArrayBuilder(map_path)

    else:
        location='mem'
        halomap.bytearraybuilder = WinMemoryByteArrayBuilder('halo')

    # class ByteArray:
    #  -- Denotes a region of bytes.
    #  -- Provides methods for reading and writing datatypes such as byte[], int, float, and string.
    #  -- Uses relative internal offsets, so two ByteArrayes which wrap the same data at different
    #     locations will always appear identical.
    #
    #  Call `ByteArray(offset, size)` to create new instances:
    #           offset: location within the source medium
    #             size: number of bytes enclosed
    #
    ByteArray = halomap.bytearraybuilder.CreateByteArray

    if location == 'mem':
        # Force Halo to render video even when window is deselected
        exe_offset = 0x400000
        wmkillHandler_offset = exe_offset + 0x142538
        ByteArray(wmkillHandler_offset, 4).WriteBytes(0, Array[Byte]((0xe9, 0x91, 0x00, 0x00)))

    # fetch the necessary struct layouts
    MapHeader = plugin_classes['map_header']
    IndexHeader = plugin_classes['index_header']
    TagHeader = plugin_classes['tag_header']

    # load the map header
    map_header = MapHeader(
                        ByteArray(
                            offset={'file': 0, 'mem': 0x6A8154}[location],
                            size=MapHeader.struct_size),
                        halomap)

    # load the index header
    index_header = IndexHeader(
                        ByteArray(
                            offset={'file': map_header.index_offset, 'mem': 0x40440000}[location],
                            size=IndexHeader.struct_size),
                        halomap)

    if location == 'file':
        # Usually the tag index directly follows the index header. However, some forms of
        # map protection move the tag index to other locations.
        index_offset = map_header.index_offset + index_header.primary_magic - 0x40440000

        # On disk, we need to use a magic value to convert raw pointers into file offsets.
        # This magic value is based on the tag index's location within the file, since the
        # tag index always appears at the same place in memory.
        halomap.magic = index_header.primary_magic - index_offset

    elif location == 'mem':
        # Almost always 0x40440028, unless the map has been protected in a specific way.
        index_offset = index_header.primary_magic

        # In memory, offsets are just raw pointers and require no adjustment.
        halomap.magic = 0

    # load all tag headers from the index
    tag_headers = [TagHeader(
                        ByteArray(
                            offset=TagHeader.struct_size * i + index_offset,
                            size=TagHeader.struct_size),
                        halomap) for i in range(index_header.tag_count)]

    # tag metadata can recurse to unknown places, but at least we know where they start
    meta_offsets = sorted(tag_header.meta_offset_raw for tag_header in tag_headers)

    if location == 'file': # the BSP's meta has an offset of 0, skip it
        bsp_offset = meta_offsets.pop(0)

    elif location == 'mem': # the BSP's meta has a very large, distant offset, skip it
        bsp_offset = meta_offsets.pop()

    # to calculate sizes, we need the offset to the end (does not point to a tag)
    meta_offsets.append(meta_offsets[0] + map_header.metadata_size)

    #  [0, 10, 40, 60] from location offsets...
    #   [10, 30, 20]   we can calculate sizes...
    #                  but instead of an ordered list, key based on the start offset
    meta_sizes = {start: (end - start) for start, end in zip(meta_offsets[:-1], meta_offsets[1:])}

    # just give BSP's meta a size of zero for now
    meta_sizes.update({bsp_offset: 0})

    name_maxlen = 256 # not sure what the actual limit is; just picking some value

    # HaloTags can load their own name and metadata, so just give them the starting ByteArrayes
    tags = [HaloTag(
                tag_header,
                ByteArray(
                    offset=tag_header.name_offset_raw - halomap.magic,
                    size=name_maxlen),
                ByteArray(
                    offset=tag_header.meta_offset_raw - halomap.magic,
                    size=meta_sizes[tag_header.meta_offset_raw]),
                halomap) for tag_header in tag_headers]

    for entry in tag_headers:
        if entry.ident == 3797811434:
            # ghost
            Weapon = plugin_classes['weap']
            return Weapon(ByteArray(entry.meta_offset_raw, Weapon.struct_size), halomap)

    return index_header
