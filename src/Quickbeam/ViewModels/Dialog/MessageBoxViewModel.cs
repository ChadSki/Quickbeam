﻿using System.Collections.Generic;
using Quickbeam.Dialogs;
using Quickbeam.Models;

namespace Quickbeam.ViewModels.Dialog
{
	public class MessageBoxViewModel : Base
	{
		public MessageBoxViewModel(string title, string message, List<MetroMessageBox.MessageBoxButton> buttons)
		{
			Title = title;
			Message = message;
			Buttons = buttons;
		}

		public string Title
		{
			get { return _title; }
			set { SetField(ref _title, value); }
		}
		private string _title;

		public string Message
		{
			get { return _message; }
			set { SetField(ref _message, value); }
		}
		private string _message;

		public List<MetroMessageBox.MessageBoxButton> Buttons
		{
			get { return _buttons; }
			set { SetField(ref _buttons, value); }
		}
		private List<MetroMessageBox.MessageBoxButton> _buttons;
	}
}
