using System.Windows;
using Quickbeam.ViewModels.Dialog;

namespace Quickbeam.Dialogs.Controls
{
	/// <summary>
	/// Interaction logic for MetroMessageBoxWindow.xaml
	/// </summary>
	public partial class MetroPluginRevisionViewerWindow
	{
		public MetroPluginRevisionViewerWindow(PluginRevisionViewerViewModel viewModel)
		{
			InitializeComponent();

			DataContext = viewModel;
			Title = WindowTitle = viewModel.Title;
		}

		private void ExitButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void WindowCloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
