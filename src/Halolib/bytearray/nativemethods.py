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

import clr
import clrtype
from System import Array, Byte, Boolean, Int32, IntPtr, UInt32

class NativeMethods(object):
    __metaclass__ = clrtype.ClrClass

    from System.Runtime.InteropServices import DllImportAttribute, PreserveSigAttribute
    DllImport = clrtype.attribute(DllImportAttribute)
    PreserveSig = clrtype.attribute(PreserveSigAttribute)

    @staticmethod
    @DllImport("kernel32.dll")
    @PreserveSig()
    @clrtype.accepts(UInt32, Boolean, UInt32)
    @clrtype.returns(IntPtr)
    def OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId): raise RuntimeError("this should not get called")

    @staticmethod
    @DllImport("kernel32.dll")
    @PreserveSig()
    @clrtype.accepts(IntPtr, IntPtr, Array[Byte], Int32, Int32)
    @clrtype.returns(Boolean)
    def ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, dwSize, lpNumberOfBytesRead): raise RuntimeError("this should not get called")

    @staticmethod
    @DllImport("kernel32.dll")
    @PreserveSig()
    @clrtype.accepts(IntPtr, IntPtr, Array[Byte], Int32, Int32)
    @clrtype.returns(Boolean)
    def WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, dwSize, lpNumberOfBytesRead): raise RuntimeError("this should not get called")