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

class ByteAccess(object):
    """Cordons off an area of bytes. Within a ByteAccess, offsets are
    relative, and attempting to read or write outside the encapsulated
    area will raise an Exception.

    ByteAccess is an abstract class, written so that only _read_bytes and
    _write_bytes need to be implemented, usually along with a custom
    constructor.
    """
    def __init__(self, offset, size):
        self.offset = offset
        self.size = size

    def addr_within_bounds(self, addr):
        return (0 <= addr <= self.size) or (self.offset <= addr <= (self.offset + self.size))

    def create_subaccess(self, offset, size):
        if not self.addr_within_bounds(offset + size):
            raise Exception("Cannot allocate past end of Access. %s, attempted %d-%d" %( str(self), offset, offset + size))
        return self.__class__(self.offset + offset, size)

    def __str__(self):
        return 'offset:%d size:%d' % (self.offset, self.size)

    def read_all_bytes(self):
        return self.read_bytes(0, self.size)

    def read_bytes(self, offset, size):
        if not self.addr_within_bounds(offset + size):
            raise Exception("Cannot read past end of Access. %s, attempted %d-%d" %( str(self), offset, offset + size))
        return self._read_bytes(offset, size)

    def _read_bytes(self, offset, size):
        raise Exception("Reading not implemented in abstract class")

    def write_bytes(self, to_write, offset):
        if not self.addr_within_bounds(offset + len(to_write)):
            raise Exception("Cannot write past end of Access. %s, attempted %d-%d" %( str(self), offset, offset + len(to_write)))
        self._write_bytes(to_write, offset)

    def _write_bytes(self, to_write, offset):
        raise Exception("Writing not implemented in abstract class")
