using Quickbeam.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quickbeam.Views
{
    public partial class Home
    {
        private HomeViewModel ViewModel { get; set; }
        private WindowInteropHelper Helper { get; set; }

        public Home()
        {
            InitializeComponent();
            DataContext = ViewModel = new HomeViewModel();
            ViewModel.MainPage = new MainPage();
            Helper = new WindowInteropHelper(this);
            var tab = new LayoutAnchorable { Title = "Start Page", Content = new MainPage() };
            DocumentManager.Children.Add(tab);
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = !ViewModel.MainPage.Close();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            ViewModel.OnStateChanged(WindowState, e);
            base.OnStateChanged(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            OnStateChanged(null);

            base.OnSourceInitialized(e);
        }

        #region Resizing

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        private const int WmSyscommand = 0x112;
        private const int ScSize = 61440;

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(Helper.Handle, WmSyscommand, (IntPtr)(ScSize + direction), IntPtr.Zero);
        }

        private void ResizeTop(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.Top); }

        private void ResizeBottom(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.Bottom); }

        private void ResizeLeft(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.Left); }

        private void ResizeRight(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.Right); }

        private void ResizeTopLeft(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.TopLeft); }

        private void ResizeTopRight(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.TopRight); }

        private void ResizeBottomLeft(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.BottomLeft); }

        private void ResizeBottomRight(object sender, MouseButtonEventArgs e) { ResizeWindow(ResizeDirection.BottomRight); }

        #endregion Resizing
    }
}
