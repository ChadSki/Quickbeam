using Quickbeam.Native;
using Quickbeam.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Linq;

namespace Quickbeam.Helpers
{
    public class SublWindow : HwndHost
    {
        private IntPtr _hwndHost;
        private Process _sublProcess;
        private string _sublPath = Path.GetFullPath(@"SublimeText3\sublime_text.exe");
        private int _sublWidth = 200;
        private int _sublHeight = 200;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // iterate processes named sublime_text.exe and kill those based out of our path
            foreach (var subl_exe in Process.GetProcessesByName("sublime_text"))
            {
                if (subl_exe.MainModule.FileName == Path.GetFullPath(_sublPath))
                {
                    subl_exe.Kill();
                }
            }

            var psi = new ProcessStartInfo(_sublPath) { UseShellExecute = false };

            // prepend bundled Python to path
            var quickbeamDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pythonDir = Path.Combine(quickbeamDir, "Python34");
            psi.EnvironmentVariables["PATH"] = string.Format(@"{0};{0}\DLLs;{0}\Scripts;{1}",
                pythonDir, psi.EnvironmentVariables["PATH"]);

            _sublProcess = Process.Start(psi);
            _sublProcess.EnableRaisingEvents = true;
            _sublProcess.Exited += _sublProcess_Exited;

            // hide window as soon as possible
            while (_sublProcess.MainWindowHandle == IntPtr.Zero) { /* spin */ }  // TODO - bad practice to do this on the GUI thread?
            NativeMethods.ShowWindow(_sublProcess.MainWindowHandle, NativeMethods.SwHide);

            // remove control box
            int style = NativeMethods.GetWindowLong(_sublProcess.MainWindowHandle, NativeMethods.GwlStyle)
                        & ~NativeMethods.WsCaption & ~NativeMethods.WsThickframe;
            NativeMethods.SetWindowLong(_sublProcess.MainWindowHandle, NativeMethods.GwlStyle, style);

            // create host window
            _hwndHost = NativeMethods.CreateWindowEx(
                0, "static", null, NativeMethods.WsChild | NativeMethods.WsClipChildren,
                0, 0, _sublWidth, _sublHeight, hwndParent.Handle,
                IntPtr.Zero, IntPtr.Zero, 0);

            // reveal and relocate into host window
            NativeMethods.SetParent(_sublProcess.MainWindowHandle, _hwndHost);
            NativeMethods.ShowWindow(_sublProcess.MainWindowHandle, NativeMethods.SwShow);

            // resize
            NativeMethods.SetWindowPos(_sublProcess.MainWindowHandle, IntPtr.Zero, 0, 0,
                _sublWidth, _sublHeight, NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);

            return new HandleRef(this, _hwndHost);
        }

        protected override void OnWindowPositionChanged(Rect r)
        {
            _sublWidth = (int)r.Width;
            _sublHeight = (int)r.Height;

            NativeMethods.SetWindowPos(_hwndHost,
                IntPtr.Zero, 0, 0, _sublWidth, _sublHeight, NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);

            NativeMethods.SetWindowPos(_sublProcess.MainWindowHandle,
                IntPtr.Zero, 0, 0, _sublWidth, _sublHeight, NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);
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
