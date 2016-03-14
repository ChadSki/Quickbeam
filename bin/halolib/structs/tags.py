# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.
"""TODO"""

from basicstruct import field
from halolib.structs.halostruct import define_halo_struct
from halolib.structs import halofield


tag_types = {}
# type: Dict[str, HaloMapStruct]

tag_types['unknown'] = define_halo_struct(struct_size=32,
    rawdata=field.RawData(offset=0, length=32))

tag_types['bipd'] = define_halo_struct(struct_size=0x450,
    model=halofield.TagReference(offset=0x28),
    animation=halofield.TagReference(offset=0x38),
    collision=halofield.TagReference(offset=0x70),
    physics=halofield.TagReference(offset=0x80),
    turn_speed=field.Float32(offset=0x2F0),
    jump_velocity=field.Float32(offset=0x3B4),
    melee_damage=halofield.TagReference(offset=0x288),
    weapons=halofield.StructArray(offset=0x2D8, struct_size=36,
        held_weapon=halofield.TagReference(offset=0x0)))


tag_types['effe'] = define_halo_struct(struct_size=0x700,
    events=halofield.StructArray(offset=0x34, struct_size=68,
        parts=halofield.StructArray(offset=0x2C, struct_size=104,
            spawned_object=halofield.TagReference(offset=0x18))))


tag_types['proj'] = define_halo_struct(struct_size=0x248,
    model=halofield.TagReference(offset=0x28),
    animation=halofield.TagReference(offset=0x38),
    collision=halofield.TagReference(offset=0x70),
    physics=halofield.TagReference(offset=0x80),
    initial_velocity=field.Float32(offset=0x1E4),
    final_velocity=field.Float32(offset=0x1E8))


tag_types['vehi'] = define_halo_struct(struct_size=0x3F0,
    model=halofield.TagReference(offset=0x28),
    animation=halofield.TagReference(offset=0x38),
    collision=halofield.TagReference(offset=0x70),
    physics=halofield.TagReference(offset=0x80),
    max_forward_velocity=field.Float32(offset=0x2F8),
    max_reverse_velocity=field.Float32(offset=0x2FC),
    acceleration=field.Float32(offset=0x300),
    deceleration=field.Float32(offset=0x304),
    suspension_sound=halofield.TagReference(offset=0x3B0),
    crash_sound=halofield.TagReference(offset=0x3C0))


tag_types['weap'] = define_halo_struct(struct_size=0x504,
    model=halofield.TagReference(offset=0x28),
    animation=halofield.TagReference(offset=0x38),
    collision=halofield.TagReference(offset=0x70),
    physics=halofield.TagReference(offset=0x80),
    magazines=halofield.StructArray(offset=0x4F0, struct_size=112,
        rounds_recharged=field.Int16(offset=0x4),
        rounds_total_initial=field.Int16(offset=0x6),
        rounds_total_maximum=field.Int16(offset=0x8),
        rounds_loaded_maximum=field.Int16(offset=0xA)),
    triggers=halofield.StructArray(offset=0x4FC, struct_size=276,
        initial_rounds_per_second=field.Float32(offset=0x4),
        final_rounds_per_second=field.Float32(offset=0x8),
        rounds_per_shot=field.Int16(offset=0x22),
        projectiles_per_shot=field.Int16(offset=0x6E),
        projectile=halofield.TagReference(offset=0x94),
        firing_effects=halofield.StructArray(offset=0x108, struct_size=132,
            fire_effect=halofield.TagReference(offset=0x24),
            misfire_effect=halofield.TagReference(offset=0x34),
            no_ammo_effect=halofield.TagReference(offset=0x44),
            fire_damage=halofield.TagReference(offset=0x54),
            misfire_damage=halofield.TagReference(offset=0x64),
            no_ammo_damage=halofield.TagReference(offset=0x74))))
