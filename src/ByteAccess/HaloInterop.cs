using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quickbeam.ByteAccess.Memory
{
    /// <summary>
    /// Wraps Win32 APIs for reading and writing to a process' memory. Tuned
    /// specifically to Halo PC.
    /// </summary>
    public static class HaloInterop
    {
        const UInt32 ProcessAllAccess = 0x1F0FFF;

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        #region HaloPidSingleton
        private static IntPtr _haloProcessId = IntPtr.Zero;
        private static IntPtr GetHaloPid()
        {
            if (_haloProcessId == IntPtr.Zero)
            {
                Process[] processesByName = Process.GetProcessesByName("halo");
                if (processesByName.Length > 0)
                    _haloProcessId = OpenProcess(ProcessAllAccess, false, processesByName[0].Id);
            }
            return _haloProcessId;
        }
        #endregion HaloPidSingleton

        public static bool ReadHaloMemory(IntPtr offset, [Out] byte[] buffer, int length)
        {
            IntPtr haloProcessId = GetHaloPid();
            if (haloProcessId == IntPtr.Zero)
                return false;

            int bytesWritten = 0;
            bool result = ReadProcessMemory(haloProcessId, offset, buffer, length, out bytesWritten);
            return result;
        }

        public static bool WriteHaloMemory(IntPtr offset, byte[] buffer, int length)
        {
            IntPtr haloProcessId = GetHaloPid();
            if (haloProcessId == IntPtr.Zero)
                return false;

            int bytesWritten = 0;
            bool result = WriteProcessMemory(haloProcessId, offset, buffer, length, out bytesWritten);
            return result;
        }
    }

}
