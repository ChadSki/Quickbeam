using System.IO;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Quickbeam.Native;
using Quickbeam.Views;

namespace Quickbeam.Helpers
{
    public class SublWindow : HwndHost
    {
        private IntPtr _hwndHost;
        private Process _sublProcess;
        private int _sublWidth;
        private int _sublHeight;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var replPage = App.Storage.HomeWindowViewModel.AssemblyPage as MainPage;
            if (replPage == null) throw new Exception("Unable to locate ReplPage");
            _sublWidth = replPage.ViewModel.HaloWidth;
            _sublHeight = replPage.ViewModel.HaloHeight;
            MaxWidth = _sublWidth;
            MaxHeight = _sublHeight;

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
                _sublProcess = Process.Start(new ProcessStartInfo(@"C:\Users\Chad\Desktop\Sublime Text Build 3065 x64\sublime_text.exe"));
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
                NativeMethods.SetWindowPos(_sublProcess.MainWindowHandle, IntPtr.Zero, 0, 0, _sublWidth, _sublHeight,
                    NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);
            });

            return new HandleRef(this, _hwndHost);
        }

        private void _sublProcess_Exited(object sender, EventArgs e)
        {
            var replPage = App.Storage.HomeWindowViewModel.AssemblyPage as MainPage;
            if (replPage == null) return;
            replPage.Dispatcher.Invoke(replPage.RemoveHaloPage);
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
