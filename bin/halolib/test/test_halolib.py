# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import os
import unittest
from halolib import HaloMap
from halolib.structs import tag_types

def find_maps_folder():
    for drive in ('C:\\', 'D:\\', 'E:\\'):  # probably enough drives
        for arch in ('Program Files (x86)', 'Program Files'):
            maps_folder = os.path.join(
                drive, arch, 'Microsoft Games', 'Halo', 'MAPS')
            if os.path.exists(maps_folder):
                return maps_folder
    raise FileNotFoundError('Cannot find Halo MAPS folder.')


class SimpleMapTest(object):

    def test_iterate_all_tags(self):
        print(self.map)
        print(self.map.index_header.tag_count)
        for tag in self.map.tags():
            print('{}'.format(tag))

    def test_print_known_tags(self):
        for tag_class in tag_types:
            print(tag_class)
            for tag in self.map.tags(tag_class):
                print(repr(tag))

class BloodgulchTest(SimpleMapTest, unittest.TestCase):

    def setUp(self):
        bloodgulch_path = os.path.join(
            find_maps_folder(), 'bloodgulch.map')
        self.map = HaloMap.from_file(bloodgulch_path)

class MemoryMapTest(SimpleMapTest, unittest.TestCase):

    def setUp(self):
        self.map = HaloMap.from_hpc()

if __name__ == '__main__':
    unittest.main()
