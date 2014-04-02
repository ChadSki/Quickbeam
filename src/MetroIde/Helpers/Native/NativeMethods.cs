using System;
using System.Runtime.InteropServices;

namespace MetroIde.Helpers.Native
{
    internal static class NativeMethods
    {
        public const int
            GwlStyle = -16,
            SwShow = 5,
            SwpNoActivate = 0x0010,
            SwpNoZOrder = 0x0004,
            SwpNoCopyBits = 0x0100,
            WsCaption = 0x00C00000,
            WsChild = 0x40000000,
            WsClipChildren = 0x02000000,
            WsClipSiblings = 0x04000000,
            WsThickframe = 0x00040000,
            WsVisible = 0x10000000;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, int style,
                                                     int x, int y, int width, int height,
                                                     IntPtr hwndParent, IntPtr hMenu, IntPtr hInst,
                                                     [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    }
}
