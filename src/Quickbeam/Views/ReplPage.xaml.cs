using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Quickbeam.Dialogs;
using Quickbeam.Models;
using Quickbeam.ViewModels;

namespace Quickbeam.Views
{
	/// <summary>
	/// Interaction logic for StartPage.xaml
	/// </summary>
	public partial class ReplPage : IAssemblyPage
	{
		public ReplPageViewModel ViewModel { get; private set; }

		public ReplPage()
		{
			InitializeComponent();

            DataContext = ViewModel = new ReplPageViewModel();
		}

		public bool Close()
		{
			return true;
		}
	}
}
