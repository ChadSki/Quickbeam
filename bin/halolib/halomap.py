# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import re
from byteaccess import open_file, open_process
from .structs import IndexHeader, MapHeader, TagHeader
from .halotag import HaloTag

class HaloMap(object):

    """Represents a single Halo mapfile and everything in it.

    Attributes
    ----------
    map_header : HaloStruct
        todo

    index_header : HaloStruct
        todo

    tags_by_ident : Dict[int, HaloTag]
        Each tag in the map, addressable by unique id.

    context
        Data context from which the map was loaded. As a HaloMap object is edited,
        changes are written back via this context.
    """

    @staticmethod
    def from_hpc():
        """Load a map from Halo's memory. Changes immediately take effect in-game,
        but will be lost unless saved to disk.
        """
        return HaloMap(open_process('halo.exe'))

    @staticmethod
    def from_hded():
        """Load a map from Halo's memory. Changes immediately take effect in-game,
        but will be lost unless saved to disk.
        """
        return HaloMap(open_process('haloded.exe'))

    @staticmethod
    def from_file(map_path):
        """Load a map from a local mapfile. Changes immediately take effect;
        keep backups of your map files!

        Parameters
        ----------
        map_path : string
            Location of the map on disk to open for editing.
        """
        return HaloMap(open_file(map_path))

    def __init__(self, map_access):
        """Load a map from the given context (either a file or Halo's memory).

        Parameters
        ----------
        context
            Where to read map data and write changes.
        """
        super().__init__()
        self.map_access = map_access

        map_header = MapHeader(map_access, {'file': 0, 'mem': 0x6A8154})

        index_header = IndexHeader(map_access,
            {'file': map_header.index_offset, 'mem': 0x40440000})

        # Almost always 0x40440028 (map protection may change this)
        mem_index_offset = index_header.primary_magic

        # Usually the tag index directly follows the index header. However,
        # some forms of map protection move the tag index to other locations.
        file_index_offset = (index_header.primary_magic - 0x40440000
                             + map_header.index_offset)

        # On disk, we need to use a magic value to convert raw pointers into file
        # offsets. Offsets in memory are already valid pointers.
        self.magic_offset = {
            'file': index_header.primary_magic - file_index_offset,
            'mem': 0}

        tag_headers = [
            TagHeader(self,
                {'file': TagHeader.struct_size * i + file_index_offset,
                 'mem': TagHeader.struct_size * i + mem_index_offset})
            for i in range(index_header.tag_count)]

        # build associative tag collection
        tags_by_ident = {tag_header.ident: HaloTag(tag_header, self)
                         for tag_header in tag_headers}
        # type: Dict[int, HaloTag]

        # save references to stuff
        map_header.property_changed.add(
            lambda *args, **kwargs: self.property_changed(*args, **kwargs))
        self.map_header = map_header
        index_header.property_changed.add(
            lambda *args, **kwargs: self.property_changed(*args, **kwargs))
        self.index_header = index_header
        self.tags_by_ident = tags_by_ident

    def tag(self, tag_class='', *name_fragments):
        """Find a tag by name and class. Returns the first tag to match all
        criteria, or None.

        Parameters
        ----------
        tag_class : string
            Filter your search by a full or partial tag class name. Use the
            empty string to include all classes in your search. Regular
            expressions are enabled.

            Examples: '', 'bipd', 'obje', 'proj|weap|vehi', 's(cex|chi|gla)'

        name_fragments : tuple of string, optional
            Each fragment is independently searched for in tag names. Only a tag
            which contains all fragments will be returned. Regular expressions
            are enabled.

            Example fragments: '', 'cyborg', 'plasma.*(rifle|pistol)'
        """
        return next(self.tags(tag_class, *name_fragments), None)

    def tags(self, tag_class='', *name_fragments):
        """Filter tags by name and class. Same as self.tag(), but iterates
        through all tags which match the search criteria.
        """
        def match(tag):
            return (any((re.search(tag_class, tag.first_class),
                         re.search(tag_class, tag.second_class),
                         re.search(tag_class, tag.third_class)))
                    and all(re.search(regex, tag.name)
                           for regex in name_fragments))

        return filter(match, self.tags_by_ident.values())

