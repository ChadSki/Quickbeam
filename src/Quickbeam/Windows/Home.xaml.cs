using Quickbeam.ViewModels;
using Quickbeam.Views;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Quickbeam.Windows
{
	/// <summary>
	/// Interaction logic for Home.xaml
	/// </summary>
	public partial class Home
	{
		public HomeViewModel ViewModel { get; private set; }
        private WindowInteropHelper Helper { get; set; }

		public Home()
		{
			InitializeComponent();
			App.Storage.HomeWindow = this;

			ViewModel = new HomeViewModel();
			DataContext = App.Storage.HomeWindowViewModel = ViewModel;
			ViewModel.AssemblyPage = new ReplPage();
            Helper = new WindowInteropHelper(this);

			Closing += OnClosing;
		}

		private static void OnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			cancelEventArgs.Cancel = !App.Storage.HomeWindowViewModel.AssemblyPage.Close();
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
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

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

        private const int WM_SYSCOMMAND = 0x112;

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(Helper.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
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
