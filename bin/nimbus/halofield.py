# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import basicstruct
from basicstruct.field import BasicField
from .halostruct import define_halo_struct

def add_offsets(offset_dict, edit_fn):
    """TODO"""
    return {where: edit_fn(offset)
            for where, offset in offset_dict.items()}

class HaloField(BasicField):

    """Fields that require a halomap to make sense, and can count on their
    parent struct to retain a reference to one."""

    @property
    def halomap(self):
        return self.parent.halomap


class AsciizPtr(HaloField):

    """Pointer to a null-terminated string somewhere else in the mapfile."""

    # really should use some low constant with a doubling mechanism or something
    max_str_size = 260

    def __init__(self, offset, docs=""):
        super().__init__(offset, docs)
        self.string_access = None

    def getf(self, byteaccess):
        if self.string_access is None:
            name_offset_raw = byteaccess.read_uint32(self.offset)
            name_offset = add_offsets(
                self.halomap.magic_offset,
                lambda magic: name_offset_raw - magic)
            self.string_access = self.halomap.map_access(
                name_offset, AsciizPtr.max_str_size)
        return self.string_access.read_asciiz(0, AsciizPtr.max_str_size)
        
    def setf(self, byteaccess):
        raise NotImplementedError()


class TagReference(HaloField):

    """Semantic link to a HaloTag."""

    def __init__(self, *, offset, loneid=False, docs=""):
        super().__init__(offset, docs)

        # LoneIDs (idents just by themselves) need no adjustment.
        if not loneid:
            # This is a full reference, but we only care to read the ident
            self.offset += 12  # which is located 12 bytes inside

    def getf(self, byteaccess):
        ident = byteaccess.read_uint32(self.offset)
        if ident == 0xFFFFFFFF:
            return None
        try:
            return self.halomap.tags_by_ident[ident]  # the referenced tag
        except KeyError:
            print("keyerror")
            return None  # we wanted a tag that wasn't there =(

    def setf(self, byteaccess, value):
        # when value is None, write Halo's version of null (-1)
        # otherwise, write the tag's ident, not the tag itself
        byteaccess.write_uint32(self.offset,
                                0xFFFFFFFF if value is None
                                else value.ident)


class StructArray(HaloField):

    """A pointer to an array of Halo structs somewhere else in the mapfile."""

    def __init__(self, *, offset, docs="", **kwargs):
        super().__init__(offset, docs)
        self.struct_type = define_halo_struct(**kwargs)
        self.children = None

    def getf(self, byteaccess):
        if self.children is None:
            count = byteaccess.read_uint32(self.offset)
            array_offset_raw = byteaccess.read_uint32(self.offset + 4)

            array_offset = add_offsets(
                self.halomap.magic_offset,
                lambda magic: array_offset_raw - magic)

            size = self.struct_type.struct_size

            if count > 1024:  # something's fucky
                raise RuntimeError('{} structs in struct_array?!'.format(count))

            # get accesses and build structs around them, then save for later
            self.children = [
                self.struct_type(self.halomap,
                    add_offsets(array_offset, lambda offset: offset + i * size))
                for i in range(count)]
        return self.children

    def setf(self, byteaccess, value):
        raise NotImplementedError(
            'Reassigning entire struct arrays is not yet supported.')
