# Copyright (c) 2016, Chad Zawistowski
# All rights reserved.
#
# This software is free and open source, released under the 2-clause BSD
# license as detailed in the LICENSE file.
"""
Common interface for reading and writing binary data to files and processes.

Within a ByteAccess, offsets are relative. This means that ByteAccesses which
wrap the same data always appear and behave identically, regardless of where
that data actually is. This is useful when operations need to be performed on
identical data from different locations in the same source, or from different
sources altogether.

Example usage:

    import byteaccess

    if location == 'file':
        MyThingAccess = byteaccess.byteaccess_for_file('filename.txt')
    elif location == 'mem'
        MyThingAccess = byteaccess.byteaccess_for_process('processname')

    foo = MyThingAccess(offset, size)
    foo.write_bytes(0, b'somedata')    # write data to offsets within the ByteAccess
    foo.read_bytes(0, 6)  #=> b'someda'  # read any length of data from any offset
"""

__version__ = '0.4.0'
__all__ = ['FileContext', 'MemContext']


from .file import open_file

import platform
if platform.system() == 'Windows':
    from .process import open_process

elif platform.system() == 'Darwin':
    raise NotImplementedError("Mac support not yet available")

else:
    raise NotImplementedError("Unsupported platform.")
