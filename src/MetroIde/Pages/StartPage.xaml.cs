using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using MetroIde.Helpers;
using MetroIde.Dialogs;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace MetroIde.Pages
{
    /// <summary>
    ///     Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage
    {
        public StartPage()
        {
            InitializeComponent();

            // Set DataContext
            DataContext = App.MetroIdeStorage.MetroIdeSettings;

            // Setup Combo Boxes
            ComboBoxAccents.ItemsSource = Enum.GetValues(typeof(Settings.Accents));
        }

        public void LoadRecentItem(object sender, RoutedEventArgs e)
        {
            var senderEntry = (Settings.RecentFileEntry) ((Button) sender).Tag;

            if (senderEntry == null) return;

            if (!File.Exists(senderEntry.FilePath))
            {
                if (
                    MetroMessageBox.Show("File Not Found",
                        "That file can't be found on your computer. Would you like it to be removed from the recents list?",
                        MetroMessageBox.MessageBoxButtons.YesNo) != MetroMessageBox.MessageBoxResult.Yes) return;
                RecentFiles.RemoveEntry(senderEntry);
                UpdateRecents();
            }
            else
                switch (senderEntry.FileType)
                {
                    case Settings.RecentFileType.Cache:
                        App.MetroIdeStorage.MetroIdeSettings.HomeWindow.AddCacheTabModule(senderEntry.FilePath);
                        break;
                    default:
                        MetroMessageBox.Show("wut.",
                            "This content type doesn't even exist, how did you manage that?");
                        break;
                }
        }

        public void UpdateRecents()
        {
            PanelRecents.Children.Clear();

            if (App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Count != 0)
            {
                int recentsCount = 0;
                foreach (Settings.RecentFileEntry entry in App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents)
                {
                    if (recentsCount > 9)
                        break;

                    var btnRecent = new Button
                    {
                        Tag = entry,
                        Style = (Style) FindResource("TabActiveButtons"),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        ToolTip = entry.FilePath
                    };
                    btnRecent.Click += LoadRecentItem;

                    if (entry.FileType == Settings.RecentFileType.Cache)
                    {
                        btnRecent.Content = string.Format("{0} - {1}", entry.FileGame, entry.FileName.Replace("_", "__"));
                        PanelRecents.Children.Add(btnRecent);
                    }

                    recentsCount++;
                }
            }
            else if (App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Count == 0)
                PanelRecents.Children.Add(new TextBlock
                {
                    Text = "It's lonely in here, get modding ;)",
                    Style = (Style) FindResource("GenericTextblock"),
                    Margin = new Thickness(20, 0, 0, 0)
                });
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

        private void LaunchHaloFullscreen_OnClick(object sender, RoutedEventArgs e)
        {
            string haloExePath = App.MetroIdeStorage.MetroIdeSettings.HaloExePath;
            if (File.Exists(haloExePath))
            {
                string haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory != null && Directory.Exists(haloDirectory))
                {
                    int width, height;
                    if (App.MetroIdeStorage.MetroIdeSettings.AutoDetectFullscreenResolution)
                    {
                        width = 800;
                        height = 600;
                    }
                    else
                    {
                        width = App.MetroIdeStorage.MetroIdeSettings.HaloFullWidth;
                        height = App.MetroIdeStorage.MetroIdeSettings.HaloFullHeight;
                    }
                    Process.Start(new ProcessStartInfo(haloExePath)
                    {
                        WorkingDirectory = haloDirectory,
                        Arguments = string.Format(@"-console -vidmode {0},{1},{2}", width, height, 60),
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
    }
}