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
        private static string SublPath { get { return Path.GetFullPath(@".\SublimeText3\sublime_text.exe"); } }

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

            var quickbeamWorkingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var sublStartInfo = new ProcessStartInfo(SublPath)
            {
                UseShellExecute = false,
                WorkingDirectory = quickbeamWorkingDir
            };
            // prepend bundled Python to path so SublimeREPL can find and launch it.
            sublStartInfo.EnvironmentVariables["PATH"] =
                string.Format(@"{0};{0}\DLLs;{0}\Scripts;{1}",
                              Path.Combine(quickbeamWorkingDir, "Python34"),
                              sublStartInfo.EnvironmentVariables["PATH"]);

            SublProcess = Process.Start(sublStartInfo);
            SublProcess.EnableRaisingEvents = true;
            SublProcess.Exited += On_Exited;

            // hide window as soon as possible (bad practice, but sublime text launches quickly)
            while (SublProcess.MainWindowHandle == IntPtr.Zero) { /* spin */ }
            NativeApi.ShowWindow(SublProcess.MainWindowHandle, NativeApi.SwHide);

            // remove control box
            int style = NativeApi.GetWindowLong(SublProcess.MainWindowHandle, NativeApi.GwlStyle)
                        & ~NativeApi.WsCaption & ~NativeApi.WsThickframe;
            NativeApi.SetWindowLong(SublProcess.MainWindowHandle, NativeApi.GwlStyle, style);

            // create host window
            HwndHost = NativeApi.CreateWindowEx(
                0, "static", null, NativeApi.WsChild | NativeApi.WsClipChildren, 0, 0,
                SublWidth, SublHeight, hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, 0);

            // reveal and relocate into host window
            NativeApi.SetParent(SublProcess.MainWindowHandle, HwndHost);
            NativeApi.ShowWindow(SublProcess.MainWindowHandle, NativeApi.SwShow);

            // resize
            NativeApi.SetWindowPos(SublProcess.MainWindowHandle, IntPtr.Zero, 0, 0,
                SublWidth, SublHeight, NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);

            return new HandleRef(this, HwndHost);
        }

        private void On_Exited(object sender, EventArgs e)
        {
            // avoid strange loops
            SublProcess.Exited -= On_Exited;

            // gui updates must happen on the main thread
            Application.Current.Dispatcher.Invoke(Dispose);
        }

        protected override void OnWindowPositionChanged(Rect r)
        {
            SublWidth = (int)r.Width;
            SublHeight = (int)r.Height;

            NativeApi.SetWindowPos(HwndHost, IntPtr.Zero, 0, 0,
                SublWidth, SublHeight, NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);

            NativeApi.SetWindowPos(SublProcess.MainWindowHandle, IntPtr.Zero, 0, 0,
                SublWidth, SublHeight, NativeApi.SwpNoZOrder | NativeApi.SwpNoActivate);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!SublProcess.HasExited)
                    SublProcess.Kill();
                SublProcess.Close();
            }
            catch (InvalidOperationException) { /* shhh */ }
            base.Dispose(disposing);
        }
    }
}
