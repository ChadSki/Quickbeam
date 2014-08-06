using System.IO;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Quickbeam.Low.ByteArray;
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
            var replPage = App.Storage.HomeWindowViewModel.AssemblyPage as ReplPage;
            if (replPage == null) throw new Exception("Unable to locate ReplPage");
            _haloWidth = replPage.ViewModel.HaloWidth;
            _haloHeight = replPage.ViewModel.HaloHeight;
            MaxWidth = _haloWidth;
            MaxHeight = _haloHeight;

            _hwndHost = NativeMethods.CreateWindowEx(
                0, "static", "",
                NativeMethods.WsChild | NativeMethods.WsVisible | NativeMethods.WsClipChildren,
                0, 0,
                _haloWidth, _haloHeight,
                hwndParent.Handle,
                IntPtr.Zero,
                IntPtr.Zero,
                0);

            // This code uses Thread.Sleep, so finish on a background thread
            Task.Factory.StartNew(() =>
            {
                var haloExePath = App.Storage.Settings.HaloExePath;
                if (!File.Exists(haloExePath)) return;

                var haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory == null || !Directory.Exists(haloDirectory)) return;

                _haloProcess = Process.Start(new ProcessStartInfo(haloExePath)
                {
                    WorkingDirectory = haloDirectory,
                    Arguments = string.Format(@"-console -window -vidmode {0},{1},{2}", _haloWidth, _haloHeight, RefreshRate),
                    WindowStyle = ProcessWindowStyle.Minimized
                });
                _haloProcess.EnableRaisingEvents = true;
                _haloProcess.Exited += _haloProcess_Exited;

                _haloProcess.WaitForInputIdle();
                Thread.Sleep(1800);

                // remove control box
                int style = NativeMethods.GetWindowLong(_haloProcess.MainWindowHandle, NativeMethods.GwlStyle);
                style = style & ~NativeMethods.WsCaption & ~NativeMethods.WsThickframe;
                NativeMethods.SetWindowLong(_haloProcess.MainWindowHandle, NativeMethods.GwlStyle, style);

                // reveal and relocate into our window
                NativeMethods.SetParent(_haloProcess.MainWindowHandle, _hwndHost);
                NativeMethods.ShowWindow(_haloProcess.MainWindowHandle, NativeMethods.SwShow);

                // resize
                NativeMethods.SetWindowPos(_haloProcess.MainWindowHandle, IntPtr.Zero, 0, 0, _haloWidth, _haloHeight,
                    NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);

                // force video rendering
                const int exeOffset = 0x400000;
                const int wmkillHandlerOffset = exeOffset + 0x142538;
                var wmkillHandler = new WinMemoryByteArrayBuilder(@"halo").CreateByteArray(wmkillHandlerOffset, 4);
                wmkillHandler.WriteBytes(0, new byte[] { 0xe9, 0x91, 0x00, 0x00 });
            });

            return new HandleRef(this, _hwndHost);
        }

        private void _haloProcess_Exited(object sender, EventArgs e)
        {
            var replPage = App.Storage.HomeWindowViewModel.AssemblyPage as ReplPage;
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
