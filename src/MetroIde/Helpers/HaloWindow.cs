using System.IO;
using MetroIde.Helpers.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Quickbeam.Low.ByteAccess;

namespace MetroIde.Helpers
{
    public class HaloWindow : HwndHost
    {
        private IntPtr _hwndHost;
        private Process _haloProcess;
        private readonly int _hostHeight;
        private readonly int _hostWidth;

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
                string haloExePath = App.MetroIdeStorage.MetroIdeSettings.HaloExePath;
                if (!File.Exists(haloExePath)) return;

                string haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory == null || !Directory.Exists(haloDirectory)) return;

                _haloProcess = Process.Start(new ProcessStartInfo(haloExePath)
                {
                    WorkingDirectory = haloDirectory,
                    Arguments = string.Format(@"-console -window -vidmode {0},{1},60", 800, 600),
                    WindowStyle = ProcessWindowStyle.Minimized
                });

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

                // force video rendering
                const int exeOffset = 0x400000;
                const int wmkillHandlerOffset = exeOffset + 0x142538;
                var wmkillHandler = new WinMemoryByteAccess(wmkillHandlerOffset, 4);
                wmkillHandler.WriteBytes(0, new byte[] { 0xe9, 0x91, 0x00, 0x00 });
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
