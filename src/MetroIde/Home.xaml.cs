using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using AvalonDock.Controls;
using AvalonDock.Layout;
using MetroIde.Dialogs;
using MetroIde.Helpers.Native;
using MetroIde.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;

namespace MetroIde
{
    /// <summary>
    ///     Interaction logic for Home.xaml
    /// </summary>
    public partial class Home
    {
        private int _lastDocumentIndex = -1;

        #region Startup

        public Home()
        {
            InitializeComponent();

            UpdateTitleText("");
            UpdateStatusText("Ready");

            AddTabModule(TabGenre.StartPage);
            //AddTabModule(TabGenre.LobbyPage);

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
        }

        #endregion

        #region MenuButtons
        private void menuOpenCacheFile_Click(object sender, RoutedEventArgs e)
        {
            OpenContentFile(ContentTypes.Map);
        }
        private void menuViewStartPage_Click(object sender, RoutedEventArgs e)
        {
            AddTabModule(TabGenre.StartPage);
        }
        private void menuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            MetroAbout.Show();
        }
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

            Width = xAdjust > MinWidth ? xAdjust : MinWidth;
        }

        public void ResizeBottomThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = Height + e.VerticalChange;

            Height = yAdjust > MinHeight ? yAdjust : MinHeight;
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
            LobbyPage,
            HaloPage
        }

        /// <summary>
        ///     Add a new Blam Cache Editor Container
        /// </summary>
        /// <param name="cacheLocation">Path to the Blam Cache File</param>
        public void AddCacheTabModule(string cacheLocation)
        {
            // Check Map isn't already open
            foreach (LayoutContent tab in CenterDockManager.Children.Where(tab => tab.ContentId == cacheLocation))
            {
                CenterDockManager.SelectedContentIndex = CenterDockManager.IndexOfChild(tab);
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
            CenterDockManager.Children.Add(newCacheTab);
            CenterDockManager.SelectedContentIndex = CenterDockManager.IndexOfChild(newCacheTab);
        }

        public void AddTabModule(TabGenre tabG, bool singleInstance = true)
        {
            LayoutContent tab;
            switch (tabG)
            {
                case TabGenre.StartPage:
                    tab = new LayoutAnchorable { Title = "Start Page", Content = new StartPage() };
                    break;

                case TabGenre.LobbyPage:
                    tab = new LayoutAnchorable {Title = "HaloMD Lobby", Content = new LobbyPage()};
                    break;

                default:
                    return;
            }

            // Select the single tab rather than create a new one
            if (singleInstance)
                foreach (LayoutContent tabb in CenterDockManager.Children.Where(tabb => tabb.Title == tab.Title))
                {
                    CenterDockManager.SelectedContentIndex = CenterDockManager.IndexOfChild(tabb);
                    return;
                }

            CenterDockManager.Children.Add(tab);
            CenterDockManager.SelectedContentIndex = CenterDockManager.IndexOfChild(tab);
        }

        public void AddHaloViewport()
        {
            var haloTab = new LayoutAnchorable { Title = "Halo Viewport", Content = new HaloPage() };
            HaloDockManager.Children.Add(haloTab);

            HaloDockManager.SelectedContentIndex = HaloDockManager.IndexOfChild(haloTab);
            RightDock.DockMinWidth = App.MetroIdeStorage.MetroIdeSettings.HaloDockedWidth;
            HaloDock.DockMinHeight = App.MetroIdeStorage.MetroIdeSettings.HaloDockedHeight + 20;
        }

        public void AddEditTab()
        {
            var editTab = new LayoutAnchorable { Title = "Tag Editor", Content = new EditPage() };
            LobbyDockManager.Children.Add(editTab);
        }

        private void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (CenterDockManager.SelectedContentIndex == _lastDocumentIndex) return;

            // Selection Changed, lets do dis
            var tab = CenterDockManager.SelectedContent;
            if (tab != null)
            {
                UpdateTitleText(tab.Title.Replace("__", "_").Replace(".map", "")); //TODO check this is necessary
            }
            else
            {
                CenterDockManager.SelectedContentIndex = 0;
                UpdateTitleText("");
            }
            _lastDocumentIndex = CenterDockManager.SelectedContentIndex;
        }

        #endregion

        #region Public byteArray Modifiers

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