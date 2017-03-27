# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

from .basebyteaccess import BaseByteAccess
from mmap import mmap

def open_file(filepath):
    """Return a class for reading and writing to a specific file.
    Instantiate ByteAccess objects to encapsulate specific regions.

    filepath -- Full path to the file which will be opened.

    Example usage:
        ScratchpadAccess = byteaccess_for_file('scratchpad.bin')
        foo = ScratchpadAccess(offset, size)
        bar = ScratchpadAccess(other_offset, other_size)
        foo.write_bytes(0, bar.read_all_bytes())
        ScratchpadAccess.close()
    """

    class FileByteAccess(BaseByteAccess):

        """Read/write bytes to a file on disk."""

        file_handle = open(filepath, 'r+b')
        mmap_f = mmap(file_handle.fileno(), 0)

        def __init__(self, offset, size):
            if isinstance(offset, dict):
                offset = offset['file']
            super().__init__(offset, size)

        @classmethod
        def close(self):
            """Close the file and invalidate all child ByteAccesses."""
            FileByteAccess.mmap_f.close()
            FileByteAccess.file_handle.close()

        def _read_bytes(self, offset, size):
            begin = self.offset + offset
            end = begin + size
            buf = FileByteAccess.mmap_f[begin:end]

            if len(buf) != size:
                raise RuntimeError(('requested {} bytes but got only buffer:{}\n'
                                    '    self.offset:{} request_offset:{}\n'
                                    '    request_begin:{} request_end:{}')
                                    .format(size, buf,
                                            self.offset, offset,
                                            begin, end))
            return buf

        def _write_bytes(self, offset, to_write):
            begin = self.offset + offset
            end = begin + len(to_write)
            self.mmap_f[begin:end] = to_write

    return FileByteAccess
