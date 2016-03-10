﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using AvalonDock.Layout;
using Microsoft.Win32;
using MetroIde.Helpers;
using MetroIde.Helpers.Native;
using MetroIde.Helpers.Net;
using MetroIde.Dialogs;
using MetroIde.Pages;

namespace MetroIde
{
    /// <summary>
    ///     Interaction logic for Home.xaml
    /// </summary>
    public partial class Home
    {
        private int _lastDocumentIndex = -1;
        
        #region Startup

        public bool ProcessCommandLineArgs(IList<string> args)
        {
            if (args != null && args.Count > 1)
            {
                string[] commandArgs = args.Skip(1).ToArray();
                if (commandArgs[0].StartsWith("assembly://"))
                    commandArgs[0] = commandArgs[0].Substring(11).Trim('/');

                // Decide what to do
                Activate();
                switch (commandArgs[0].ToLower())
                {
                    case "open":
                        // Determine type of file, and start it up, yo
                        if (commandArgs.Length > 1)
                            StartupDetermineType(commandArgs[1]);
                        break;

                    case "about":
                        // Show About
                        menuHelpAbout_Click(null, null);
                        break;

                    case "settings":
                        // Show Settings
                        menuOpenSettings_Click(null, null);
                        break;

                    default:
                        return true;
                }
            }

            return true;
        }

        public Home()
        {
            InitializeComponent();

            UpdateTitleText("");
            UpdateStatusText("Ready");

            AddTabModule(TabGenre.StartPage);

            // Do sidebar Loading stuff
            //SwitchXBDMSidebarLocation(App.AssemblyStorage.AssemblySettings.applicationXBDMSidebarLocation);
            //XBDMSidebarTimerEvent();

            // Set width/height/state from last session
            if (!double.IsNaN(App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeHeight) &&
                App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeHeight > MinHeight)
                Height = App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeHeight;
            if (!double.IsNaN(App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeWidth) &&
                App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeWidth > MinWidth)
                Width = App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeWidth;

            WindowState = App.MetroIdeStorage.MetroIdeSettings.ApplicationSizeMaximize
                ? WindowState.Maximized
                : WindowState.Normal;
            Window_StateChanged(null, null);

            AllowDrop = true;
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow = this;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource != null)
                hwndSource.AddHook(WindowProc);

            ProcessCommandLineArgs(Environment.GetCommandLineArgs());
        }

        private void StartupDetermineType(string path)
        {
            try
            {
                if (File.Exists(path))
                {/*
                    // Magic Check
                    string magic;
                    using (var stream = new EndianReader(File.OpenRead(path), Endian.BigEndian))
                        magic = stream.ReadAscii(0x04).ToLower();

                    switch (magic)
                    {
                        case "head":
                        case "daeh":
                            // Map File
                            AddCacheTabModule(path);
                            return;
                    }*/
                }

                MetroMessageBox.Show("Unable to find file", "The selected file could no longer be found");
            }
            catch (Exception ex)
            {
                MetroException.Show(ex);
            }
        }

        #endregion


        #region MenuButtons
        // File
        private void menuOpenCacheFile_Click(object sender, RoutedEventArgs e)
        {
            OpenContentFile(ContentTypes.Map);
        }

        // View
        private void menuViewStartPage_Click(object sender, RoutedEventArgs e)
        {
            AddTabModule(TabGenre.StartPage);
        }

        private void menuOpenSettings_Click(object sender, EventArgs e)
        {
            AddTabModule(TabGenre.Settings);
        }

        // Help
        private void menuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            MetroAbout.Show();
        }

        // Goodbye Sweet Evelyn
        private void menuCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region ChromeButtons

        private void btnActionMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnActionRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void btnActionMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void btnActionClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Resizing

        public void ResizeBottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height + e.VerticalChange;
            var xAdjust = Width + e.HorizontalChange;

            Width = xAdjust > MinWidth ? xAdjust : MinWidth;
            Height = yAdjust > MinHeight ? yAdjust : MinHeight;
        }

        public void ResizeBottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height + e.VerticalChange;
            var xAdjust = Width - e.HorizontalChange;

            if (xAdjust > MinWidth)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            else
            {
                var diff = Width - MinWidth;
                if (diff > 0)
                {
                    Left += diff; // mirror following change
                    Width -= diff; // Width = MinWidth
                }
                else
                {
                    Left -= diff; // mirror following change
                    Width += diff; // Width = MinWidth
                }
            }

            Height = yAdjust > MinHeight ? yAdjust : MinHeight;
        }

        public void ResizeTopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height - e.VerticalChange;
            var xAdjust = Width + e.HorizontalChange;

            Width = xAdjust > MinWidth ? xAdjust : MinWidth;

            if (yAdjust > MinHeight)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            else
            {
                var diff = Height - MinHeight;
                if (diff > 0)
                {
                    Top += diff;	// mirror following change
                    Height -= diff; // Height = MinHeight
                }
                else
                {
                    Top -= diff;	// mirror following change
                    Height += diff; // Height = MinHeight
                }
            }
        }

        public void ResizeTopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height - e.VerticalChange;
            var xAdjust = Width - e.HorizontalChange;

            if (xAdjust > MinWidth)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            else
            {
                var diff = Width - MinWidth;
                if (diff > 0)
                {
                    Left += diff; // mirror following change
                    Width -= diff; // Width = MinWidth
                }
                else
                {
                    Left -= diff; // mirror following change
                    Width += diff; // Width = MinWidth
                }
            }

            if (yAdjust > MinHeight)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            else
            {
                var diff = Height - MinHeight;
                if (diff > 0)
                {
                    Top += diff;	// mirror following change
                    Height -= diff; // Height = MinHeight
                }
                else
                {
                    Top -= diff;	// mirror following change
                    Height += diff; // Height = MinHeight
                }
            }
        }

        public void ResizeRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var xAdjust = Width + e.HorizontalChange;

            if (xAdjust > MinWidth)
            {
                Width = xAdjust;
            }
            else
            {
                Width = MinWidth;
            }
        }

        public void ResizeBottomThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height + e.VerticalChange;

            if (yAdjust > MinHeight)
            {
                Height = yAdjust;
            }
            else
            {
                Height = MinHeight;
            }
        }

        public void ResizeLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var xAdjust = Width - e.HorizontalChange;

            if (xAdjust > MinWidth)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            else
            {
                var diff = Width - MinWidth;
                if (diff > 0)
                {
                    Left += diff; // mirror following change
                    Width -= diff; // Width = MinWidth
                }
                else
                {
                    Left -= diff; // mirror following change
                    Width += diff; // Width = MinWidth
                }
            }
        }

        public void ResizeTopThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height - e.VerticalChange;

            if (yAdjust > MinHeight)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            else
            {
                var diff = Height - MinHeight;
                if (diff > 0)
                {
                    Top += diff;	// mirror following change
                    Height -= diff; // Height = MinHeight
                }
                else
                {
                    Top -= diff;	// mirror following change
                    Height += diff; // Height = MinHeight
                }
            }
        }

        #endregion

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    BorderFrame.BorderThickness = new Thickness(1, 1, 1, 23);
                    BtnActionRestore.Visibility = Visibility.Collapsed;
                    BtnActionMaximize.Visibility =
                        HomeResizing.Visibility = Visibility.Visible;
                    break;
                case WindowState.Maximized:
                    BorderFrame.BorderThickness = new Thickness(0, 0, 0, 23);
                    BtnActionRestore.Visibility = Visibility.Visible;
                    BtnActionMaximize.Visibility =
                        HomeResizing.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        #region Maximize Workspace Workarounds

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MonitorWorkArea.MinMaxInfo) Marshal.PtrToStructure(lParam, typeof (MonitorWorkArea.MinMaxInfo));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            const int monitorDefaulttonearest = 0x00000002;
            IntPtr monitor = MonitorWorkArea.MonitorFromWindow(hwnd, monitorDefaulttonearest);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MonitorWorkArea.MonitorInfo();
                MonitorWorkArea.GetMonitorInfo(monitor, monitorInfo);
                MonitorWorkArea.Rect rcWorkArea = monitorInfo.rcWork;
                MonitorWorkArea.Rect rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);

                /*
				mmi.ptMaxPosition.x = Math.Abs(scrn.Bounds.Left - scrn.WorkingArea.Left);
				mmi.ptMaxPosition.y = Math.Abs(scrn.Bounds.Top - scrn.WorkingArea.Top);
				mmi.ptMaxSize.x = Math.Abs(scrn.Bounds.Right - scrn.WorkingArea.Left);
				mmi.ptMaxSize.y = Math.Abs(scrn.Bounds.Bottom - scrn.WorkingArea.Top);
				*/
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        #endregion

        #region Content Management

        public enum ContentTypes
        {
            Map
        }

        private readonly Dictionary<ContentTypes, ContentFileHandler> _contentFileHandlers = new Dictionary
            <ContentTypes, ContentFileHandler>
        {
            {
                ContentTypes.Map, new ContentFileHandler(
                    "Quickbeam - Open Blam Cache File",
                    "Blam Cache File (*.map)|*.map",
                    (home, file) => home.AddCacheTabModule(file))
            },
        };

        /// <summary>
        ///     Open a new Blam Engine File
        /// </summary>
        /// <param name="contentType">Type of content to open</param>
        public void OpenContentFile(ContentTypes contentType)
        {
            ContentFileHandler handler;
            if (!_contentFileHandlers.TryGetValue(contentType, out handler)) return;

            var ofd = new OpenFileDialog
            {
                Title = handler.Title,
                Filter = handler.Filter,
                Multiselect = handler.AllowMultipleFiles,
            };

            if (!(bool) ofd.ShowDialog(this)) return;

            if (handler.AllowMultipleFiles)
                foreach (string file in ofd.FileNames)
                    handler.FileHandler(this, file);
            else
                handler.FileHandler(this, ofd.FileName);
        }

        private class ContentFileHandler
        {
            public readonly Action<Home, string> FileHandler;

            public ContentFileHandler(string title, string filter, Action<Home, string> handler,
                bool allowMultipleFiles = true)
            {
                Title = title;
                Filter = filter;
                AllowMultipleFiles = allowMultipleFiles;
                FileHandler = handler;
            }

            public string Title { get; private set; }
            public string Filter { get; private set; }
            public bool AllowMultipleFiles { get; private set; }
        };

        #endregion

        #region Tabs

        public enum TabGenre
        {
            StartPage,
            Settings,
            NetworkPoking,
            PluginGenerator,
            Welcome,
            PluginConverter,

            MemoryManager,
            VoxelConverter,
            PostGenerator,
            HaloPage
        }

        public void ClearTabs()
        {
            DocumentManager.Children.Clear();
        }

        /// <summary>
        ///     Add a new Blam Cache Editor Container
        /// </summary>
        /// <param name="cacheLocation">Path to the Blam Cache File</param>
        public void AddCacheTabModule(string cacheLocation)
        {
            // Check Map isn't already open
            foreach (LayoutContent tab in DocumentManager.Children.Where(tab => tab.ContentId == cacheLocation))
            {
                DocumentManager.SelectedContentIndex = DocumentManager.IndexOfChild(tab);
                return;
            }

            var newCacheTab = new LayoutDocument
            {
                ContentId = cacheLocation,
                Title = "",
                ToolTip = cacheLocation
            };
            /*newCacheTab.Content = new HaloMap(cacheLocation, newCacheTab,
                App.AssemblyStorage.AssemblySettings.HalomapTagSort);*/
            DocumentManager.Children.Add(newCacheTab);
            DocumentManager.SelectedContentIndex = DocumentManager.IndexOfChild(newCacheTab);
        }

        public void AddTabModule(TabGenre tabG, bool singleInstance = true)
        {
            LayoutContent tab;
            switch (tabG)
            {
                case TabGenre.StartPage:
                    tab = new LayoutAnchorable { Title = "Start Page", Content = new StartPage() };
                    break;
                case TabGenre.Welcome:
                    tab = new LayoutAnchorable { Title = "Welcome", Content = new WelcomePage() };
                    break;
                case TabGenre.Settings:
                    tab = new LayoutAnchorable { Title = "Settings", Content = new SettingsPage() };
                    break;
                default:
                    return;
            }

            // Select the single tab rather than create a new one
            if (singleInstance)
                foreach (LayoutContent tabb in DocumentManager.Children.Where(tabb => tabb.Title == tab.Title))
                {
                    DocumentManager.SelectedContentIndex = DocumentManager.IndexOfChild(tabb);
                    return;
                }

            DocumentManager.Children.Add(tab);
            DocumentManager.SelectedContentIndex = DocumentManager.IndexOfChild(tab);
        }

        public void AddHaloViewport()
        {
            // select existing, if available
            foreach (var tabb in RightDockManager.Children.Where(tabb => tabb.Title == "Halo Viewport"))
            {
                RightDockManager.SelectedContentIndex = RightDockManager.IndexOfChild(tabb);
                return;
            }
            var tab = new LayoutAnchorable { Title = "Halo Viewport", Content = new HaloPage() };
            RightDockManager.Children.Add(tab);
            RightDockManager.SelectedContentIndex = RightDockManager.IndexOfChild(tab);
        }

        private void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (DocumentManager.SelectedContentIndex != _lastDocumentIndex)
            {
                // Selection Changed, lets do dis
                LayoutContent tab = DocumentManager.SelectedContent;

                if (tab != null)
                    UpdateTitleText(tab.Title.Replace("__", "_").Replace(".map", ""));

                if (tab != null && tab.Title == "Start Page")
                    ((StartPage)tab.Content).UpdateRecents();

                if (tab == null)
                {
                    DocumentManager.SelectedContentIndex = 0;
                    UpdateTitleText("");
                }

                _lastDocumentIndex = DocumentManager.SelectedContentIndex;
            }
        }

        #endregion

        #region Public Access Modifiers

        private readonly DispatcherTimer _statusUpdateTimer = new DispatcherTimer();

        /// <summary>
        ///     Set the title text of Assembly
        /// </summary>
        /// <param name="title">Current Title, Assembly shall add the rest for you.</param>
        public void UpdateTitleText(string title)
        {
            string suffix = "Quickbeam";
            if (!string.IsNullOrWhiteSpace(title))
                suffix = " - " + suffix;

            Title = title + suffix;
            LblTitle.Text = title + suffix;
        }

        /// <summary>
        ///     Set the status text of Assembly
        /// </summary>
        /// <param name="status">Current Status of Assembly</param>
        public void UpdateStatusText(string status)
        {
            Status.Text = status;

            _statusUpdateTimer.Stop();
            _statusUpdateTimer.Interval = new TimeSpan(0, 0, 0, 4);
            _statusUpdateTimer.Tick += statusUpdateCleaner_Clear;
            _statusUpdateTimer.Start();
        }

        private void statusUpdateCleaner_Clear(object sender, EventArgs e)
        {
            Status.Text = "Ready";
        }

        #endregion

        #region Opacity Masking

        public int OpacityIndex;

        public void ShowMask()
        {
            OpacityIndex++;
            OpacityRect.Visibility = Visibility.Visible;
        }

        public void HideMask()
        {
            OpacityIndex--;

            if (OpacityIndex == 0)
                OpacityRect.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Drag&Drop Support

        private void HomeWindow_PreviewDrop(object sender, DragEventArgs e)
        {
        }

        private void HomeWindow_Drop(object sender, DragEventArgs e)
        {
            // FIXME: Boot into Win7, to fix this. (Win8's UAC is so fucked up... No drag and drop on win8 it seems...)
            //string[] draggedFiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
        }

        #endregion
    }
}