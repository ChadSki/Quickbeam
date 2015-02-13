using System.IO;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Quickbeam.Native;
using Quickbeam.Views;
using System.Reflection;

namespace Quickbeam.Helpers
{
    public class SublWindow : HwndHost
    {
        private IntPtr _hwndHost;
        private Process _sublProcess;

        private int _sublWidth = 200;
        private int _sublHeight = 200;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _hwndHost = NativeMethods.CreateWindowEx(
                0, "static", null,
                NativeMethods.WsChild | NativeMethods.WsClipChildren,
                0, 0,
                _sublWidth, _sublHeight,
                hwndParent.Handle,
                IntPtr.Zero,
                IntPtr.Zero,
                0);

            // This code uses Thread.Sleep, so finish on a background thread
            Task.Factory.StartNew(() =>
            {
                // Execute Sublime Text with our included version of Python
                var psi = new ProcessStartInfo(@"SublimeText3\sublime_text.exe")
                    {
                        UseShellExecute = false,
                    };
                var quickbeamDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var pythonDir = Path.Combine(quickbeamDir, "Python34");
                psi.EnvironmentVariables["PATH"] = string.Format(@"{0}\;{0}\DLLs;{0}\Scripts;{1}",
                    pythonDir, psi.EnvironmentVariables["PATH"]);

                _sublProcess = Process.Start(psi);
                _sublProcess.EnableRaisingEvents = true;
                _sublProcess.Exited += _sublProcess_Exited;

                _sublProcess.WaitForInputIdle();
                Thread.Sleep(1800);

                // remove control box
                int style = NativeMethods.GetWindowLong(_sublProcess.MainWindowHandle, NativeMethods.GwlStyle);
                style = style & ~NativeMethods.WsCaption & ~NativeMethods.WsThickframe;
                NativeMethods.SetWindowLong(_sublProcess.MainWindowHandle, NativeMethods.GwlStyle, style);

                // reveal and relocate into our window
                NativeMethods.SetParent(_sublProcess.MainWindowHandle, _hwndHost);
                NativeMethods.ShowWindow(_sublProcess.MainWindowHandle, NativeMethods.SwShow);

                // resize
                NativeMethods.SetWindowPos(_sublProcess.MainWindowHandle, IntPtr.Zero, 0, 0,
                    _sublWidth, _sublHeight, NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);
            });

            return new HandleRef(this, _hwndHost);
        }

        protected override void OnWindowPositionChanged(Rect r)
        {
            _sublWidth = (int)r.Width;
            _sublHeight = (int)r.Height;

            NativeMethods.SetWindowPos(_hwndHost, IntPtr.Zero, 0, 0, _sublWidth, _sublHeight,
                NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);

            // this might still be under construction
            if (_sublProcess != null && _sublProcess.MainWindowHandle != null)
            {
                NativeMethods.SetWindowPos(_sublProcess.MainWindowHandle, IntPtr.Zero, 0, 0, _sublWidth, _sublHeight,
                    NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);
            }
        }

        private void _sublProcess_Exited(object sender, EventArgs e)
        {
            var mainPage = App.Storage.HomeWindowViewModel.MainPage as MainPage;
            if (mainPage == null) return;
            mainPage.Dispatcher.Invoke(mainPage.RemoveSublPage);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (_sublProcess == null) return;
            _sublProcess.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_sublProcess == null) return;
            _sublProcess.Close();
        }
    }
}
