# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

from basicstruct.struct import define_basic_struct
from basicstruct import field
from halolib.structs.halostruct import define_halo_struct
from halolib.structs import halofield

MapHeader = \
    define_basic_struct(struct_size=132,
        integrity=field.Ascii(offset=0, length=4, reverse=True),
        game_version=field.UInt32(offset=4),
        map_size=field.UInt32(offset=4),
        index_offset=field.UInt32(offset=16),
        metadata_size=field.UInt32(offset=20),
        map_name=field.Asciiz(offset=32, maxlength=32),
        map_build=field.Asciiz(offset=64, maxlength=64),
        map_type=field.UInt32(offset=128))

IndexHeader = \
    define_basic_struct(struct_size=40,
        primary_magic=field.UInt32(offset=0),
        base_tag_ident=field.UInt32(offset=4),
        map_id=field.UInt32(offset=8),
        tag_count=field.UInt32(offset=12),
        verticie_count=field.UInt32(offset=16),
        verticie_offset=field.UInt32(offset=20),
        indicie_count=field.UInt32(offset=24),
        indicie_offset=field.UInt32(offset=28),
        model_data_length=field.UInt32(offset=32),
        integrity=field.Ascii(offset=36, length=4, reverse=True))

TagHeader = \
    define_halo_struct(struct_size=32,
        first_class=field.Ascii(offset=0, length=4, reverse=True),
        second_class=field.Ascii(offset=4, length=4, reverse=True),
        third_class=field.Ascii(offset=8, length=4, reverse=True),
        ident=field.UInt32(offset=12),
        name=halofield.AsciizPtr(offset=16),
        meta_offset_raw=field.UInt32(offset=20),
        indexed=field.UInt32(offset=24))

