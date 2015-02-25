using Quickbeam.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Quickbeam.Views
{
    public class SublWindow : HwndHost
    {
        private IntPtr HwndHost { get; set; }
        private Process SublProcess { get; set; }
        private static string SublPath { get { return Path.GetFullPath(@"SublimeText3\sublime_text.exe"); } }

        private int SublWidth { get; set; }
        private int SublHeight { get; set; }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // iterate processes named sublime_text.exe and kill those based out of our path
            foreach (var sublExe in Process.GetProcessesByName("sublime_text")
                .Where(sublExe => sublExe.MainModule.FileName == Path.GetFullPath(SublPath)))
            {
                sublExe.Kill();
            }

            var psi = new ProcessStartInfo(SublPath) { UseShellExecute = false };

            // prepend bundled Python to path
            var quickbeamDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (quickbeamDir == null) throw new FileNotFoundException("Can't find the Quickbeam directory?");
            psi.EnvironmentVariables["QUICKBEAMDIR"] = quickbeamDir;
            var pythonDir = Path.Combine(quickbeamDir, "Python34");
            psi.EnvironmentVariables["PATH"] =
                string.Format(@"{0};{0}\DLLs;{0}\Scripts;{1}", pythonDir, psi.EnvironmentVariables["PATH"]);

            SublProcess = Process.Start(psi);
            SublProcess.EnableRaisingEvents = true;
            SublProcess.Exited += delegate { Application.Current.Dispatcher.Invoke(MainPage.RemoveSublPage); };

            // hide window as soon as possible
            while (SublProcess.MainWindowHandle == IntPtr.Zero) { /* spin */ }  // TODO - bad practice to do this on the GUI thread?
            NativeApi.ShowWindow(SublProcess.MainWindowHandle, NativeApi.SwHide);

            // remove control box
            int style = NativeApi.GetWindowLong(SublProcess.MainWindowHandle, NativeApi.GwlStyle)
                        & ~NativeApi.WsCaption & ~NativeApi.WsThickframe;
            NativeApi.SetWindowLong(SublProcess.MainWindowHandle, NativeApi.GwlStyle, style);

            // create host window
            HwndHost = NativeApi.CreateWindowEx(
                0, "static", null, NativeApi.WsChild | NativeApi.WsClipChildren,
                0, 0, SublWidth, SublHeight, hwndParent.Handle,
                IntPtr.Zero, IntPtr.Zero, 0);

            // reveal and relocate into host window
            NativeApi.SetParent(SublProcess.MainWindowHandle, HwndHost);
            NativeApi.ShowWindow(SublProcess.MainWindowHandle, NativeApi.SwShow);

            // resize
            NativeApi.SetWindowPos(SublProcess.MainWindowHandle, IntPtr.Zero, 0, 0,
                SublWidth, SublHeight, NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);

            return new HandleRef(this, HwndHost);
        }

        protected override void OnWindowPositionChanged(Rect r)
        {
            SublWidth = (int)r.Width;
            SublHeight = (int)r.Height;

            NativeApi.SetWindowPos(HwndHost,
                IntPtr.Zero, 0, 0, SublWidth, SublHeight, NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);

            NativeApi.SetWindowPos(SublProcess.MainWindowHandle,
                IntPtr.Zero, 0, 0, SublWidth, SublHeight, NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (SublProcess == null) return;
            SublProcess.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (SublProcess == null) return;
            SublProcess.Close();
        }
    }
}
