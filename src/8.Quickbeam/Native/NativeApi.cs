﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Quickbeam.Native
{
    #region Structs

    [StructLayout(LayoutKind.Sequential)]
    public struct MinMaxInfo
    {
        public Point ptReserved;
        public Point ptMaxSize;
        public Point ptMaxPosition;
        public Point ptMinTrackSize;
        public Point ptMaxTrackSize;
    };

    /// <remarks>
    /// This is a class (not a struct) so cbSize can be automatically initialized.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MonitorInfo
    {
        public uint cbSize = (uint)Marshal.SizeOf(typeof(MonitorInfo));
        public Rectangle rcMonitor = new Rectangle();
        public Rectangle rcWork = new Rectangle();
        public uint dwFlags = 0;
    }

    #endregion Structs

    internal static class NativeApi
    {
        public const int
            GwlStyle = -16,
            HtClient = 1,
            HtCaption = 2,
            SwHide = 0,
            SwShow = 5,
            SwpNoActivate = 0x10,
            SwpNoZOrder = 0x4,
            SwpNoCopyBits = 0x100,
            WmncHitTest = 0x84,
            WsCaption = 0xC00000,
            WsChild = 0x40000000,
            WsClipChildren = 0x2000000,
            WsClipSiblings = 0x4000000,
            WsThickframe = 0x40000,
            WsVisible = 0x10000000;

        [DllImport("user32", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName,
                                                   int style, int x, int y, int width, int height,
                                                   IntPtr hwndParent, IntPtr hMenu, IntPtr hInst,
                                                   [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32", SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

        [DllImport("user32", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("User32", SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                               int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    }
}
