using System;
using System.ComponentModel;
using Quickbeam.Views;
using Quickbeam.ViewModels;

namespace Quickbeam.Windows
{
	/// <summary>
	/// Interaction logic for Home.xaml
	/// </summary>
	public partial class Home
	{
		public HomeViewModel ViewModel { get; private set; }

		public Home()
		{
			InitializeComponent();
			App.Storage.HomeWindow = this;

			ViewModel = new HomeViewModel();
			DataContext = App.Storage.HomeWindowViewModel = ViewModel;
			ViewModel.AssemblyPage = new ReplPage();

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
	}
}
