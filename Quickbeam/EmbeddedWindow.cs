using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace Quickbeam
{
    public class EmbeddedWindow : IDisposable
    {
        private Process _process;

        private const int GWL_STYLE = -16;
        private const int SW_SHOW = 5;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_NOZORDER = 0x0004;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);


        public EmbeddedWindow(Window hostWin, string exePath, string workingDirectory, string arguments, string windowTitle)
        {
            WindowInteropHelper wndHelp = new WindowInteropHelper(hostWin);
            ProcessStartInfo psi = new ProcessStartInfo(exePath);
            psi.WorkingDirectory = workingDirectory;
            psi.Arguments = arguments;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            _process = Process.Start(psi);

            // Wait for the child window to invisibly load, then relocate into our window
            _process.WaitForInputIdle();
            Thread.Sleep(2000);
            IntPtr hWnd = FindWindow(null, windowTitle);
            ShowWindow(hWnd, SW_SHOW);
            EnableWindow(hWnd, true);
            SetParent(_process.MainWindowHandle, wndHelp.Handle);

            // remove control box
            int style = GetWindowLong(_process.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            SetWindowLong(_process.MainWindowHandle, GWL_STYLE, style);

            Resize(800, 600);
        }

        public void Resize(int width, int height, int x = 6, int y = 42)
        {
            SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
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
