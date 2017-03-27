# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.

from .basebyteaccess import BaseByteAccess
from .windowsinterop import find_process
import ctypes
from ctypes import (c_ulong, c_char_p)
k32 = ctypes.windll.kernel32



def open_process(process_name):
    """Define a ByteAccess type for a specific process.

    process_name -- The name of the target process. e.g. 'notepad' or 'notepad.exe'

    Example usage:
        import byteaccess
        NotepadAccess = byteaccess.open_process('notepad')
        foo = NotepadAccess(offset, size)
        bar = NotepadAccess(other_offset, other_size)
        foo.write_bytes(0, bar.read_all_bytes())
        NotepadAccess.close()
    """

    class WinMemByteAccess(BaseByteAccess):

        """Read/write bytes to a specific process's memory."""

        PROCESS_ALL_ACCESS = 0x1F0FFF
        process_entry = find_process(process_name.encode('ascii'))
        process = k32.OpenProcess(PROCESS_ALL_ACCESS, False,
                                  process_entry.th32ProcessID)

        def __init__(self, offset, size):
            if isinstance(offset, dict):
                offset = offset['mem']
            super().__init__(offset, size)

        @classmethod
        def close(self):
            """Close the process and invalidate all child ByteAccesses."""
            k32.CloseHandle(WinMemByteAccess.process)

        def _read_bytes(self, offset, size):
            address = self.offset + offset
            buf = ctypes.create_string_buffer(size)
            bytesRead = c_ulong(0)
            if k32.ReadProcessMemory(WinMemByteAccess.process, address,
                                     buf, size, ctypes.byref(bytesRead)):
                return bytes(buf)
            else:
                raise OSError("Failed to read memory. offset:{0} size:{1}"
                              .format(offset, size))

        def _write_bytes(self, offset, to_write):
            address = self.offset + offset
            buf = c_char_p(to_write)
            size = len(to_write)
            bytesWritten = c_ulong(0)
            if k32.WriteProcessMemory(WinMemByteAccess.process, address,
                                      buf, size, ctypes.byref(bytesWritten)):
                return  # Success!
            else:
                raise OSError("Failed to write memory. offset:{0} size:{1}"
                              .format(offset, size))

    return WinMemByteAccess
