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

"""halomap.py

Defines the HaloMap class, as well as functions for loading maps location disk or memory.
"""
import re
import mmap
from pnpc import PyNotifyPropertyChanged
from halolib.haloaccess import access_over_file, access_over_process
from halolib.halostruct import plugin_classes, load_plugins
from halolib.halotag import HaloTag

class HaloMap(PyNotifyPropertyChanged('asdf', 'map_header', 'index_header', 'map_magic')):
    def __init__(self, mmap_f=None, f=None, *args, **kwargs):
        self.mmap_f = mmap_f
        self.file = f
        super(HaloMap, self).__init__(*args, **kwargs)

    def init(self, map_header, index_header, taglist, map_magic, file=None):
        self._map_header = map_header
        self._index_header = index_header
        self._map_magic = map_magic
        self._asdf = 4
        self.file = file
        self.tags = {tag.ident: tag for tag in taglist}

    def __str__(self):
        return '[map_header]%s\n[index_header]%s' % (str(self.map_header), str(self.index_header))

    def __repr__(self):
        answer = str(self)
        answer += '\nTag Index:\n'
        for each in self.get_tags():
            answer += str(each) + '\n'
        return answer

    def get_tag(self, first_class='', *name_fragments):
        try:
            return next(self.get_tags(first_class, *name_fragments))
        except StopIteration:
            return None
    
    def get_tags(self, first_class='', *name_fragments):
        for tag in self.tags.values():
            if first_class == '' or re.search(first_class, tag.first_class):
                if all((regex == '' or re.search(regex, tag.name)) for regex in name_fragments):
                    yield tag
    
    def close(self):
        if self.mmap_f != None:
            self.mmap_f.close()
            self.mmap_f = None
        if self.file != None:
            self.file.close()
            self.file = None

def load_map(map_path=None):
    if map_path != None:
        location = 'file'
        f = open(map_path, 'r+b')
        mmap_f = mmap.mmap(f.fileno(), 0)   # Memory-mapped files are addressable as memory
                                            # despite remaining on-disk. It is both faster and
                                            # easier to read/write small amounts of data to a
                                            # mmap'd file than a normally accessed one.

        ByteAccess = access_over_file(mmap_f)
        halomap = HaloMap(mmap_f, f)

        # offsets known ahead of time
        map_header_offset = 0

    else:
        location='mem'
        ByteAccess = access_over_process('halo.exe')
        halomap = HaloMap()

        # offsets known ahead of time
        map_header_offset = 0x6A8154
        index_header_offset = 0x40440000
        exe_offset = 0x400000
        wmkillHandler_offset = exe_offset + 0x142538

        # Force Halo to render video even when window is deselected
        if True: #if fix_video_render:
            ByteAccess(wmkillHandler_offset, 4).write_bytes(b'\xe9\x91\x00\x00', 0)

    # ensure plugins are loaded
    if len(plugin_classes) == 0:
        load_plugins()

    # mapfile structures
    MapHeader = plugin_classes['map_header']
    IndexHeader = plugin_classes['index_header']
    IndexEntry = plugin_classes['index_entry']

    if location == 'mem':
        # runtime-only structures
        ObjectTable = plugin_classes['object_table']
        PlayerTable = plugin_classes['player_table']

        object_table = ObjectTable(ByteAccess(0x400506B4, 64), 0, halomap)
        print(object_table)
        
        player_table = ByteAccess(0x402AAF94, 64)
        print(player_table.read_all_bytes())

    map_header = MapHeader(
                    ByteAccess(
                        map_header_offset,
                        MapHeader.struct_size),
                    0, # the map header never requires magic
                    halomap)

    if location == 'file':
        index_header_offset = map_header.index_offset

    index_header = IndexHeader(
                    ByteAccess(
                        index_header_offset,
                        IndexHeader.struct_size),
                    0, # the index header never requires magic
                    halomap)

    if location == 'file':
        # Usually the tag index directly follows the index header. However,
        # some forms of map protection move the tag index to other locations.
        index_offset = map_header.index_offset + index_header.primary_magic - 0x40440000

        # On disk, we need to use a magic value to convert pointers into file offsets.
        # The magic value is based on the index location.
        map_magic = index_header.primary_magic - index_offset

    elif location == 'mem':
        # Almost always 0x40440028, unless the map has been protected in a specific way.
        index_offset = index_header.primary_magic

        # In memory, offsets are just raw pointers and require no adjustment.
        map_magic = 0

    index_entries = [IndexEntry(
                        ByteAccess(
                            IndexEntry.struct_size * i + index_offset,
                            IndexEntry.struct_size),
                        map_magic,
                        halomap) for i in range(index_header.tag_count)]

    meta_offsets = sorted(index_entry.meta_offset_raw for index_entry in index_entries)

    if location == 'file':
        # the BSP's meta has an offset of 0, skip it
        bsp_offset = meta_offsets.pop(0)

    elif location == 'mem':
        # the BSP's meta has a very large, distant offset, skip it
        bsp_offset = meta_offsets.pop()
    

    # to calculate sizes, we need the offset to the end
    meta_offsets.append(meta_offsets[0] + map_header.metadata_size)

    # [00, 10, 40, 55, 80] from location offsets...
    #   [10, 30, 15, 25]   we can calculate sizes...
    #                      but instead of an ordered list, key based on the start offset
    meta_sizes = {start: (end - start) for start, end in zip(meta_offsets[:-1], meta_offsets[1:])}

    # Just give BSP's meta a size of zero for now
    meta_sizes.update({bsp_offset: 0})

    tags = [HaloTag(
                index_entry,
                ByteAccess(
                    index_entry.name_offset_raw - map_magic,
                    256),
                ByteAccess(
                    index_entry.meta_offset_raw - map_magic,
                    meta_sizes[index_entry.meta_offset_raw]),
                map_magic,
                halomap) for index_entry in index_entries]

    halomap.init(map_header, index_header, tags, map_magic)
    return halomap
