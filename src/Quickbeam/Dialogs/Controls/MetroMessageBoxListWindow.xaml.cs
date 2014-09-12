﻿using System.Windows;
using Quickbeam.ViewModels.Dialog;

namespace Quickbeam.Dialogs.Controls
{
	/// <summary>
	/// Interaction logic for MetroMessageBoxListWindow.xaml
	/// </summary>
	public partial class MetroMessageBoxListWindow
	{
		public MetroMessageBoxListWindow(MessageBoxListViewModel viewModel)
		{
			Title = WindowTitle = viewModel.Title;
			InitializeComponent();

			DataContext = viewModel;
			ListBox.ItemsSource = viewModel.Items;
		}

		private void WindowCloseButton_OnClick(object sender, RoutedEventArgs e) { Close(); }
		private void CancelButton_Click(object sender, RoutedEventArgs e) { Close(); }

		private void ContinueButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
