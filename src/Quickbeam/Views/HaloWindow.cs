using Quickbeam.Helpers;
using Quickbeam.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Quickbeam.Views
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
            _haloWidth = Storage.MainPage.ViewModel.HaloWidth;
            _haloHeight = Storage.MainPage.ViewModel.HaloHeight;
            MaxWidth = _haloWidth;
            MaxHeight = _haloHeight;

            _hwndHost = NativeApi.CreateWindowEx(
                0, "static", null,
                NativeApi.WsChild | NativeApi.WsClipChildren,
                0, 0,
                _haloWidth, _haloHeight,
                hwndParent.Handle,
                IntPtr.Zero,
                IntPtr.Zero,
                0);

            // This code takes a while (spin-wait), so finish on a background thread
            Task.Factory.StartNew(() =>
            {
                _haloProcess = Process.Start(new ProcessStartInfo(HaloSettings.HaloExePath)
                {
                    WorkingDirectory = HaloSettings.HaloExeDir,
                    Arguments = string.Format(@"-console -window -vidmode {0},{1},{2}", _haloWidth, _haloHeight, RefreshRate),
                    WindowStyle = ProcessWindowStyle.Minimized
                });
                _haloProcess.EnableRaisingEvents = true;
                _haloProcess.Exited += _haloProcess_Exited;

                // wait for window
                _haloProcess.WaitForInputIdle();
                // we still might not have it yet...
                while (_haloProcess.MainWindowHandle == IntPtr.Zero) { /* spin */ }

                // remove control box
                int style = NativeApi.GetWindowLong(_haloProcess.MainWindowHandle, NativeApi.GwlStyle);
                style = style & ~NativeApi.WsCaption & ~NativeApi.WsThickframe;
                NativeApi.SetWindowLong(_haloProcess.MainWindowHandle, NativeApi.GwlStyle, style);

                // reveal and relocate into our window
                NativeApi.SetParent(_haloProcess.MainWindowHandle, _hwndHost);
                NativeApi.ShowWindow(_haloProcess.MainWindowHandle, NativeApi.SwShow);

                // resize
                NativeApi.SetWindowPos(_haloProcess.MainWindowHandle, IntPtr.Zero, 0, 0, _haloWidth, _haloHeight,
                    NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);
            });

            return new HandleRef(this, _hwndHost);
        }

        private void _haloProcess_Exited(object sender, EventArgs e)
        {
            Storage.MainPage.Dispatcher.Invoke(Storage.MainPage.RemoveHaloPage);
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
