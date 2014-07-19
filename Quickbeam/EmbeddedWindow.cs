using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Quickbeam
{
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
                //NativeMethods.ShowWindow(hWnd, NativeMethods.SW_SHOW);
                //NativeMethods.EnableWindow(hWnd, true);
                NativeMethods.SetParent(_process.MainWindowHandle, wndHelp.Handle);

                // remove control box
                //int style = NativeMethods.GetWindowLong(_process.MainWindowHandle, NativeMethods.GWL_STYLE);
                //style = style & ~NativeMethods.WS_CAPTION & ~NativeMethods.WS_THICKFRAME;
                //NativeMethods.SetWindowLong(_process.MainWindowHandle, NativeMethods.GWL_STYLE, style);
            }
            else Dispose();
        }

        public void Resize(int width, int height, int xCoor, int yCoor)
        {
            //NativeMethods.SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, xCoor, yCoor, width, height, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
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
