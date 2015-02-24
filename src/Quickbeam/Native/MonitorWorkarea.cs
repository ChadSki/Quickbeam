using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Quickbeam.Native
{
    internal static class MonitorWorkarea
    {
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxInfo
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MonitorInfo
        {
            public MonitorInfo()
            {
                cbSize = (uint) Marshal.SizeOf(typeof (MonitorInfo));
            }

            public uint cbSize;
            public Rectangle rcMonitor = new Rectangle();
            public Rectangle rcWork = new Rectangle();
            public uint dwFlags;
        }
    }
}
