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

from bytearray import ByteArray
from mmap import mmap

class FileByteArray(ByteArray):
    """Encapsulates reading and writing to a memory-mapped file on disk.
    """
    def __init__(self, offset, size, mmap_f):
        self.mmap_f = mmap_f
        super(FileAccess, self).__init__(offset, size)

    def _read_bytes(self, offset, size):
        begin = self.offset + offset
        end = begin + size
        return self.mmap_f[begin:end]

    def _write_bytes(self, to_write, offset):
        begin = self.offset + offset
        end = begin + len(to_write)
        self.mmap_f[begin:end] = to_write

class FileByteArrayBuilder(object):
    """Builder for creating multiple FileByteAccesses targeting the same file.
    """
    def __init__(self, filepath):
        self.file = open(filepath, 'r+b')
        self.mmap_f = mmap.mmap(self.file.fileno(), 0)

    def new_ByteArray(offset, size):
        return FileByteArray(offset, size, self.mmap_f)

    def close(self):
        if self.mmap_f != None:
            self.mmap_f.close()
            self.mmap_f = None
        if self.file != None:
            self.file.close()
            self.file = None
