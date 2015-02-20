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
    public class HaloWindow : HwndHost
    {
        private IntPtr _hwndHost;
        private Process _haloProcess;
        private int _haloWidth;
        private int _haloHeight;
        private const int RefreshRate = 60;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var mainPage = App.Storage.HomeWindowViewModel.MainPage as MainPage;
            if (mainPage == null) throw new Exception("Unable to locate ReplPage");

            _haloWidth = mainPage.ViewModel.HaloWidth;
            _haloHeight = mainPage.ViewModel.HaloHeight;
            MaxWidth = _haloWidth;
            MaxHeight = _haloHeight;

            var haloExePath = App.Storage.Settings.HaloExePath;
            var haloDirectory = Path.GetDirectoryName(haloExePath);
            _haloProcess = Process.Start(new ProcessStartInfo(haloExePath)
            {
                WorkingDirectory = haloDirectory,
                Arguments = string.Format(@"-console -window -vidmode {0},{1},{2}", _haloWidth, _haloHeight, RefreshRate),
                WindowStyle = ProcessWindowStyle.Minimized
            });
            _haloProcess.EnableRaisingEvents = true;
            _haloProcess.Exited += _haloProcess_Exited;

            // hide window as soon as possible
            while (_haloProcess.MainWindowHandle == IntPtr.Zero) { /* spin */ }  // TODO - bad practice to do this on the GUI thread?
            NativeMethods.ShowWindow(_haloProcess.MainWindowHandle, NativeMethods.SwHide);

            // remove control box
            int style = NativeMethods.GetWindowLong(_haloProcess.MainWindowHandle, NativeMethods.GwlStyle);
            style = style & ~NativeMethods.WsCaption & ~NativeMethods.WsThickframe;
            NativeMethods.SetWindowLong(_haloProcess.MainWindowHandle, NativeMethods.GwlStyle, style);

            // create host window
            _hwndHost = NativeMethods.CreateWindowEx(
                0, "static", null, NativeMethods.WsChild | NativeMethods.WsClipChildren,
                0, 0, _haloWidth, _haloHeight, hwndParent.Handle,
                IntPtr.Zero, IntPtr.Zero, 0);

            // reveal and relocate into our window
            NativeMethods.SetParent(_haloProcess.MainWindowHandle, _hwndHost);
            NativeMethods.ShowWindow(_haloProcess.MainWindowHandle, NativeMethods.SwShow);

            // resize
            NativeMethods.SetWindowPos(_haloProcess.MainWindowHandle, IntPtr.Zero, 0, 0, _haloWidth, _haloHeight,
                NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);

            return new HandleRef(this, _hwndHost);
        }

        private void _haloProcess_Exited(object sender, EventArgs e)
        {
            var replPage = App.Storage.HomeWindowViewModel.MainPage as MainPage;
            if (replPage == null) return;
            replPage.Dispatcher.Invoke(replPage.RemoveHaloPage);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (_haloProcess == null) return;
            _haloProcess.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_haloProcess == null) return;
            _haloProcess.Close();
        }
    }
}
