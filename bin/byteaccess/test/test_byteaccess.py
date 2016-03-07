# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import unittest
import os
import byteaccess

class BaseByteAccessTest(object):

    def test_strings(self):
        """Test some string functions of ByteAccess with overlapping access regions"""
        foo = self.ByteAccess(self.offset, 21)
        bar = self.ByteAccess(self.offset, 4)
        foo.write_bytes(0, b'test')
        assert foo.read_bytes(0, 4) == b'test'
        assert bar.read_all_bytes() == foo.read_bytes(0, 4)
        bar.write_bytes(0, b'asdf')
        assert foo.read_bytes(0, 4) == b'asdf'

    def test_numbers(self):
        """Test some number functions of ByteAccess with overlapping access regions"""
        foo = self.ByteAccess(self.offset, 21)
        baz = self.ByteAccess(self.offset + 15, 16)
        foo.write_float32(0, 2.0)
        foo.write_float64(4, 15.3)
        assert foo.read_float32(0) == 2.0
        assert foo.read_float64(4) == 15.3
        foo.write_int8(0, 12)
        foo.write_int16(1, 13)
        foo.write_int32(3, 14)
        foo.write_int64(7, 14)
        assert foo.read_int8(0) == 12
        assert foo.read_int16(1) == 13
        assert foo.read_int32(3) == 14
        assert foo.read_int64(7) == 14


class FileByteAccessTest(unittest.TestCase, BaseByteAccessTest):

    def setUp(self):
        this_folder = os.path.dirname(os.path.realpath(__file__))
        self.ByteAccess = byteaccess.open_file(os.path.join(this_folder, 'testfile.bin'))
        self.offset = 0

    def test_outside_file(self):
        """Test that reading outside the file fails as expected"""
        excepted = False
        try:
            foo = self.ByteAccess(self.offset + 9001, 12)
            foo.read_all_bytes()
        except:
            excepted = True
        assert excepted


class WinMemByteAccessTest(unittest.TestCase, BaseByteAccessTest):

    def setUp(self):
        self.ByteAccess = byteaccess.open_process('halo')
        self.index_header = 0x40440024
        self.offset = 0x6A8154  # suitable scratch area I guess?

    def test_membyteaccess(self):
        """Ensure we can read Halo's map header from memory"""
        foo = self.ByteAccess(self.index_header, 4)
        print(foo.read_all_bytes())
        assert foo.read_all_bytes() == b'sgat'
        foo.write_bytes(0, b'toof')
        assert foo.read_all_bytes() == b'toof'
        foo.write_bytes(0, b'sgat')
        assert foo.read_all_bytes() == b'sgat'


if __name__ == '__main__':
    unittest.main()
