using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Quickbeam
{
    internal static class NativeMethods
    {
        public const int GWL_STYLE = -16;
        public const int SW_SHOW = 5;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_NOZORDER = 0x0004;
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_THICKFRAME = 0x00040000;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool enabled);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    }

    public sealed class EmbeddedWindow : IDisposable
    {
        private Process _process;

        public EmbeddedWindow(Window hostWin, string exePath, string workingDirectory, string arguments, string windowTitle)
        {
            var wndHelp = new WindowInteropHelper(hostWin);
            var psi = new ProcessStartInfo(exePath);
            psi.WorkingDirectory = workingDirectory;
            psi.Arguments = arguments;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            _process = Process.Start(psi);

            // Wait for the child window to invisibly load
            _process.WaitForInputIdle();
            Thread.Sleep(2000);
            IntPtr hWnd = NativeMethods.FindWindow(null, windowTitle);

            if (hWnd != null && !_process.HasExited)
            {
                // reveal and relocate into our window
                NativeMethods.ShowWindow(hWnd, NativeMethods.SW_SHOW);
                NativeMethods.EnableWindow(hWnd, true);
                NativeMethods.SetParent(_process.MainWindowHandle, wndHelp.Handle);

                // remove control box
                int style = NativeMethods.GetWindowLong(_process.MainWindowHandle, NativeMethods.GWL_STYLE);
                style = style & ~NativeMethods.WS_CAPTION & ~NativeMethods.WS_THICKFRAME;
                NativeMethods.SetWindowLong(_process.MainWindowHandle, NativeMethods.GWL_STYLE, style);
            }
            else Dispose();
        }

        public void Resize(int width, int height, int xCoor, int yCoor)
        {
            NativeMethods.SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, xCoor, yCoor, width, height, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
        }

        public void Dispose()
        {
            if (_process != null)
            {
                _process.Refresh();
                _process.Close();
            }
        }
    }
}
