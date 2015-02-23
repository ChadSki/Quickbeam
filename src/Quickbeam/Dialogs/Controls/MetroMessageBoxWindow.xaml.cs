using System.Windows;
using System.Windows.Controls;
using Quickbeam.ViewModels;

namespace Quickbeam.Dialogs.Controls
{
	/// <summary>
	/// Interaction logic for MetroMessageBoxWindow.xaml
	/// </summary>
	public partial class MetroMessageBoxWindow
	{
		public MetroMessageBoxWindow(MessageBoxViewModel viewModel)
		{
			InitializeComponent();

			DataContext = viewModel;
			Title = WindowTitle = viewModel.Title;
		}

		public MetroMessageBox.MessageBoxButton ExitButtonType = MetroMessageBox.MessageBoxButton.Okay;

		private void ExitButton_OnClick(object sender, RoutedEventArgs e)
		{
			var button = (Button) sender;
			if (button != null && button.Tag is MetroMessageBox.MessageBoxButton)
				ExitButtonType = (MetroMessageBox.MessageBoxButton) button.Tag;

			Close();
		}

		private void WindowCloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
