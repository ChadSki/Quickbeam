import clr
import clrtype
from System import *

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
    def OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId): raise RuntimeError("this should not get called")\

    @staticmethod
    @DllImport("kernel32.dll")
    @PreserveSig()
    @clrtype.accepts(IntPtr, IntPtr, Array[Byte], Int32, Int32)
    @clrtype.returns(Boolean)
    def ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, dwSize, lpNumberOfBytesRead): raise RuntimeError("this should not get called")\

