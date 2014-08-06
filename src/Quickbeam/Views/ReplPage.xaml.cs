using System.IO;
using System.Linq;
using Quickbeam.Dialogs;
using Quickbeam.ViewModels;
using System.Windows;
using System.Windows.Forms;

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

	    private void BtnBrowseHaloExe_Click(object sender, RoutedEventArgs e)
	    {
            var dialog = new OpenFileDialog
            {
                Filter = @"Executable Files|*.exe|All Files|*.*",
                FileName = ViewModel.HaloExePath,
                Title = @"Open halo.exe"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                ViewModel.HaloExePath = dialog.FileName;
	    }

	    private void BtnLaunchHalo_Click(object sender, RoutedEventArgs e)
	    {
            var haloExePath = App.Storage.Settings.HaloExePath;
            if (File.Exists(haloExePath))
            {
                var haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory != null && Directory.Exists(haloDirectory))
                {
                    HorizontalGridSplitter.IsEnabled = false;
                    VerticalGridSplitter.IsEnabled = false;
                    HaloGrid.Children.Add(new HaloPage());
                    return;
                }
            }
            MetroMessageBox.Show("Cannot Launch Halo", "Check your halo.exe path in Settings.");
	    }

	    public void RemoveHaloPage()
	    {
            var haloPages = HaloGrid.Children.OfType<HaloPage>().ToList();
            foreach (var child in haloPages)
                HaloGrid.Children.Remove(child);
	    }
	}
}
