# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

from basicstruct import BasicStruct

class HaloStruct(BasicStruct):

    """TODO"""

    def __init__(self, byteaccess, halomap, **fields):
        object.__setattr__(self, 'halomap', halomap)
        super().__init__(byteaccess, **fields)

def define_halo_struct(struct_size, **fields):
    """Returns a constructor function for a newly defined Halo struct.

    Parameters
    ----------
    struct_size : int
        size of the struct in bytes

    **fields : Dict[str, Union[BasicField, HaloField]]
        the remaining arguments are grouped into a dictionary, with the
        argument name as the key and the argument itself (expected to be a
        field of some sort) as the value.
    """
    def finish_construction(halomap, offset):
        # enclose the bytes we need
        byteaccess = halomap.map_access(offset, struct_size)
        # build the struct interface around it
        return HaloStruct(byteaccess, halomap, **fields)

    finish_construction.struct_size = struct_size
    return finish_construction
