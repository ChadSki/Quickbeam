# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

import os
import unittest
import byteaccess
from basicstruct import Event, define_basic_struct
from basicstruct.field import (
    Ascii, Asciiz, RawData,
    Enum16, Flag, Float32, Float64,
    Int8, Int16, Int32, Int64,
    UInt8, UInt16, UInt32, UInt64)

this_folder = os.path.dirname(os.path.realpath(__file__))


class BasicStructTest(unittest.TestCase):

    """Tests each of the basic field types."""

    def setUp(self):
        TextFileAccess = byteaccess.open_file(os.path.join(this_folder, 'testfile.bin'))
        TextStruct = define_basic_struct(struct_size=0x30,
            char_array=Ascii(offset=0, length=4, reverse=True),
            char_array2=Ascii(offset=0, length=4, reverse=False),
            char_array3=Ascii(offset=4, length=1),
            c_string=Asciiz(offset=5, maxlength=19),
            bytes=RawData(offset=0x20, length=8))
        self.struct = TextStruct(TextFileAccess, 0)

    def test_ascii(self):
        print(self.struct.char_array)
        print(self.struct.char_array2)
        print(self.struct.char_array3)
        assert self.struct.char_array == 'HEAD'
        assert self.struct.char_array2 == 'DAEH'
        assert self.struct.char_array3 == 'Z'

    def test_asciiz(self):
        print(self.struct.c_string)
        assert self.struct.c_string == 'Michelangelo'

    def test_bytes(self):
        print(self.struct.bytes)
        assert self.struct.bytes == b'\xDE\xAD\xC0\xDE\xFE\xED\xC0\xBB'

class BitmapStructTest(unittest.TestCase):

    """Tests reading a tiny bitmap file."""

    def setUp(self):
        BmpFileAccess = byteaccess.open_file(os.path.join(this_folder, 'test.bmp'))

        BmpHeader = define_basic_struct(struct_size=14,
            id=Ascii(offset=0x0, length=2, docs="should be `BM`", reverse=False),
            filesize=UInt32(offset=0x2),
            pixels_offset=UInt32(offset=0xA))

        DibHeader = define_basic_struct(struct_size=40,
            header_size=UInt32(offset=0),
            width=UInt32(offset=4),
            height=UInt32(offset=8),
            num_color_panes=UInt16(offset=12),
            bits_per_pixel=UInt16(offset=14),
            buffer=RawData(offset=16, length=4),
            bmp_size=UInt32(offset=20, docs="includes padding"),
            dpi_horizontal=UInt32(offset=24),
            dpi_vertical=UInt32(offset=28),
            num_palette_colors=UInt32(offset=32),
            num_important_colors=UInt32(offset=36))

        PixelArray = define_basic_struct(struct_size=14,
            red_pixel=RawData(offset=0, length=3),
            white_pixel=RawData(offset=3, length=3),
            blue_pixel=RawData(offset=8, length=3),
            green_pixel=RawData(offset=11, length=3))

        self.bmp_header = BmpHeader(BmpFileAccess, 0)
        self.dib_header = DibHeader(BmpFileAccess, 14)
        self.pixel_array = PixelArray(BmpFileAccess, 54)

    def test_bitmap_header(self):
        assert self.bmp_header.id == 'BM'
        assert self.bmp_header.filesize == 70
        assert self.bmp_header.pixels_offset == 54

    def test_dib_header(self):
        assert self.dib_header.header_size == 40
        assert self.dib_header.width == 2
        assert self.dib_header.height == 2
        assert self.dib_header.num_color_panes == 1
        assert self.dib_header.bits_per_pixel == 24
        assert self.dib_header.bmp_size == 16
        assert self.dib_header.dpi_horizontal == 2835
        assert self.dib_header.dpi_vertical == 2835
        assert self.dib_header.num_palette_colors == 0


if __name__ == '__main__':
    unittest.main()
