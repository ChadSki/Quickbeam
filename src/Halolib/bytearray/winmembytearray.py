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

from __future__ import print_function
import clr
from System import Array, Byte, IntPtr
from System.Diagnostics import Process
from bytearray import ByteArray
from nativemethods import NativeMethods

class WinMemByteArray(ByteArray):
    """Encapsulates reading and writing to a specified process's memory.
    """
    def __init__(self, offset, size, process_id):
        super(WinMemByteArray, self).__init__(offset, size)
        self.pid = process_id

    def _read_bytes(self, offset, size):
        address = self.offset + offset
        print(size)
        buf = Array[Byte]([0 for i in range(size)])
        bytesRead = -1
        if NativeMethods.ReadProcessMemory(self.pid, IntPtr(address), buf, size, bytesRead):
            return buf.raw
        else:
            raise Exception("Failed to read memory")

    def _write_bytes(self, to_write, offset):
        address = self.offset + offset
        buf = Array[Byte](to_write)
        size = len(to_write)
        bytesWritten = -1
        if NativeMethods.WriteProcessMemory(self.pid, IntPtr(address), buf, size, bytesWritten):
            return
        else:
            raise Exception("Failed to write memory")

class WinMemByteArrayBuilder(object):
    """Builder for creating multiple WinMemoryByteAccesses targeting the same process.
    """
    from System.Diagnostics import Process

    def __init__(self, process_name):
        PROCESS_ALL_ACCESS = 0x1F0FFF
        search_results = Process.GetProcessesByName(process_name)
        if len(search_results) == 0: raise Exception("'%s' is not running" % process_name)
        self.pid = NativeMethods.OpenProcess(PROCESS_ALL_ACCESS, False, search_results[0].Id)

    def new_bytearray(self, offset, size):
        return WinMemByteArray(offset, size, self.pid)

        
        
        
        