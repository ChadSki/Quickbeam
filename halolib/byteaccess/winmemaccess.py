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

from byteaccess import ByteAccess
from ctypes import *
from ctypes.wintypes import *

__all__ = ['WinMemAccess']

class ProcessEntry32(ctypes.Structure):
     _fields_ = [("dwSize", ctypes.c_ulong),
                 ("cntUsage", ctypes.c_ulong),
                 ("th32ProcessID", ctypes.c_ulong),
                 ("th32DefaultHeapID", ctypes.c_ulong),
                 ("th32ModuleID", ctypes.c_ulong),
                 ("cntThreads", ctypes.c_ulong),
                 ("th32ParentProcessID", ctypes.c_ulong),
                 ("pcPriClassBase", ctypes.c_ulong),
                 ("dwFlags", ctypes.c_ulong),
                 ("szExeFile", ctypes.c_char * 260)]

TH32CS_SNAPPROCESS = 0x00000002
CreateToolhelp32Snapshot = windll.kernel32.CreateToolhelp32Snapshot
Process32First = windll.kernel32.Process32First
Process32Next = windll.kernel32.Process32Next
CloseHandle = windll.kernel32.CloseHandle

def process_list(hTH32Snapshot):
    """Iterates through currently open processes."""
    pe32 = ProcessEntry32()
    pe32.dwSize = ctypes.sizeof(ProcessEntry32)

    if Process32First(hTH32Snapshot, ctypes.byref(pe32)) == False:
        print("Failed getting first process.", file=sys.stderr)
        return

    while True:
        yield pe32
        if Process32Next(hTH32Snapshot, ctypes.byref(pe32)) == False:
            break

def get_process_by_name(name):
    """Returns the first running process found with the specified name, or None."""
    hTH32Snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    for each in process_list(hTH32Snapshot):
        if each.szExeFile == name:
            CloseHandle(hTH32Snapshot)
            return each

    CloseHandle(hTH32Snapshot)
    return None

PROCESS_ALL_ACCESS = 0x1F0FFF
OpenProcess = windll.kernel32.OpenProcess
ReadProcessMemory = windll.kernel32.ReadProcessMemory
WriteProcessMemory = windll.kernel32.WriteProcessMemory

class WinMemAccess(ByteAccess):
    """Encapsulates reading and writing to a specified process's memory."""

    def __init__(self, offset, size, process_name):

        # share the same process handle between all WinMemAccesses for this process
        if 'process' not in WinMemAccess.__dict__:
            halo = get_process_by_name(process_name.encode('ascii'))
            if halo == None:
                raise Exception("'%s' is not running" % process_name)

            WinMemAccess.process = OpenProcess(PROCESS_ALL_ACCESS, False, halo.th32ProcessID)

        super(WinMemAccess, self).__init__(offset, size)

    def _read_bytes(self, offset, size):
        address = self.offset + offset
        buf = create_string_buffer(size)
        bytesRead = c_ulong(0)
        if ReadProcessMemory(WinMemAccess.process, address, buf, size, byref(bytesRead)):
            return bytes(buf)
        else:
            raise Exception("Failed to read memory")

    def _write_bytes(self, to_write, offset):
        address = self.offset + offset
        buf = c_char_p(to_write)
        size = len(to_write)
        bytesWritten = c_ulong(0)
        if WriteProcessMemory(WinMemAccess.process, address, buf, size, byref(bytesWritten)):
            return
        else:
            raise Exception("Failed to write memory")