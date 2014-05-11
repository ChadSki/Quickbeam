using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using MetroIde.Helpers;
using MetroIde.Dialogs;

namespace MetroIde.Pages
{
    /// <summary>
    ///     Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage
    {
        public StartPage()
        {
            DataContext = App.MetroIdeStorage.MetroIdeSettings;
            InitializeComponent();

            // Setup Combo Boxes
            ComboBoxAccents.ItemsSource = Enum.GetValues(typeof(Settings.Accents));
        }

        private void LaunchHaloDocked_OnClick(object sender, RoutedEventArgs e)
        {
            string haloExePath = App.MetroIdeStorage.MetroIdeSettings.HaloExePath;
            if (File.Exists(haloExePath))
            {
                string haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory != null && Directory.Exists(haloDirectory))
                {
                    App.MetroIdeStorage.MetroIdeSettings.HomeWindow.AddHaloViewport();
                    return;
                }
            }
            MetroMessageBox.Show("Cannot Launch Halo", "Check your halo.exe path in Settings.");
        }

        private void LaunchHaloWindowed_OnClick(object sender, RoutedEventArgs e)
        {
            string haloExePath = App.MetroIdeStorage.MetroIdeSettings.HaloExePath;
            if (File.Exists(haloExePath))
            {
                string haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory != null && Directory.Exists(haloDirectory))
                {
                    int width = App.MetroIdeStorage.MetroIdeSettings.HaloWindowedWidth;
                    int height = App.MetroIdeStorage.MetroIdeSettings.HaloWindowedHeight;
                    Process.Start(new ProcessStartInfo(haloExePath)
                    {
                        WorkingDirectory = haloDirectory,
                        Arguments = string.Format(@"-console -windowed -vidmode {0},{1},{2}", width, height, 60),
                    });
                }
            }
        }

        private void BtnBrowseHaloExe_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = @"Executable Files|*.exe|All Files|*.*",
                FileName = App.MetroIdeStorage.MetroIdeSettings.HaloExePath,
                Title = @"Open halo.exe"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                App.MetroIdeStorage.MetroIdeSettings.HaloExePath = dialog.FileName;
        }

        private void StartPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Apply Storyboard
            var storyboardShow = (Storyboard)TryFindResource("RevealSettings");
            if (storyboardShow != null) storyboardShow.Begin();
        }

        private void EditMapMemory_OnClick(object sender, RoutedEventArgs e)
        {
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.AddEditTab();
        }
    }
}