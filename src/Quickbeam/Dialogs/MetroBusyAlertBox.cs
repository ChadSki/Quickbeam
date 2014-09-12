﻿using System.Windows;
using Quickbeam.Dialogs.Controls;
using Quickbeam.ViewModels.Dialog;

namespace Quickbeam.Dialogs
{
	public static class MetroBusyAlertBox
	{
		private const string DefaultTitle = "Background Work In Progress";
		private const string DefaultMessage = "Quickbeam is performing some background work, please wait while it finishes. Thanks.";

		public static MetroBusyAlertBoxWindow Show()
		{
			return Show(DefaultTitle, DefaultMessage);
		}

		public static MetroBusyAlertBoxWindow Show(string message)
		{
			return Show(DefaultTitle, message);
		}
		public static MetroBusyAlertBoxWindow Show(string title, string message)
		{
			var dialog = new MetroBusyAlertBoxWindow(new BusyAlertBoxViewModel(title, message))
			{
				Owner = App.Storage.HomeWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			App.Storage.HomeWindowViewModel.ShowDialog();
			dialog.Show();

			return dialog;
		}
	}
}
