using MetroIde.Helpers.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace MetroIde.Helpers
{
    public class HaloWindow : HwndHost
    {
        private IntPtr _hwndHost;
        private Process _haloProcess;
        readonly int _hostHeight;
        readonly int _hostWidth;

        public HaloWindow()
        {
            _hostHeight = 800;
            _hostWidth = 600;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _hwndHost = IntPtr.Zero;

            _hwndHost = NativeMethods.CreateWindowEx(
                0, "static", "",
                NativeMethods.WsChild | NativeMethods.WsVisible | NativeMethods.WsClipChildren,
                0, 0,
                _hostWidth, _hostHeight,
                hwndParent.Handle,
                IntPtr.Zero,
                IntPtr.Zero,
                0);

            // This code uses Thread.Sleep, so finish on a background thread
            Task.Factory.StartNew(() =>
            {
                var psi = new ProcessStartInfo(@"D:\Program Files (x86)\Microsoft Games\Halo\halo.exe")
                {
                    WorkingDirectory = @"D:\Program Files (x86)\Microsoft Games\Halo\",
                    Arguments = string.Format(@"-console -window -vidmode {0},{1},60", 800, 600),
                    WindowStyle = ProcessWindowStyle.Minimized
                };
                _haloProcess = Process.Start(psi);
                _haloProcess.WaitForInputIdle();
                Thread.Sleep(2000);

                // remove control box
                int style = NativeMethods.GetWindowLong(_haloProcess.MainWindowHandle, NativeMethods.GwlStyle);
                style = style & ~NativeMethods.WsCaption & ~NativeMethods.WsThickframe;
                NativeMethods.SetWindowLong(_haloProcess.MainWindowHandle, NativeMethods.GwlStyle, style);

                // reveal and relocate into our window
                NativeMethods.SetParent(_haloProcess.MainWindowHandle, _hwndHost);
                NativeMethods.ShowWindow(_haloProcess.MainWindowHandle, NativeMethods.SwShow);

                // resize
                NativeMethods.SetWindowPos(_haloProcess.MainWindowHandle, IntPtr.Zero, 0, 0, 800, 600, NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);

                //TODO force render trick
            });

            return new HandleRef(this, _hwndHost);
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
