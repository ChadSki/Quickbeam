using System.IO;
using System.Windows;
using System.Windows.Controls;
using ModernIde.Helpers;
using ModernIde.Dialogs;

namespace ModernIde.Windows.Pages
{
    /// <summary>
    ///     Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage
    {
        public StartPage()
        {
            InitializeComponent();

            cbClosePageOnLoad.IsChecked = App.ModernIdeStorage.ModernIdeSettings.StartpageHideOnLaunch;
            cbShowOnStartUp.IsChecked = App.ModernIdeStorage.ModernIdeSettings.StartpageShowOnLoad;
        }

        public bool Close()
        {
            return true;
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
                        App.ModernIdeStorage.ModernIdeSettings.HomeWindow.AddCacheTabModule(senderEntry.FilePath);
                        break;
                    default:
                        MetroMessageBox.Show("wut.",
                            "This content type doesn't even exist, how the fuck did you manage that?");
                        break;
                }
        }

        public void UpdateRecents()
        {
            panelRecents.Children.Clear();

            if (App.ModernIdeStorage.ModernIdeSettings.ApplicationRecents.Count != 0)
            {
                int recentsCount = 0;
                foreach (Settings.RecentFileEntry entry in App.ModernIdeStorage.ModernIdeSettings.ApplicationRecents)
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

                    if (entry.FileType == Settings.RecentFileType.Cache &&
                        App.ModernIdeStorage.ModernIdeSettings.StartpageShowRecentsMap)
                    {
                        btnRecent.Content = string.Format("{0} - {1}", entry.FileGame, entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }
                    else if (entry.FileType == Settings.RecentFileType.Blf &&
                             App.ModernIdeStorage.ModernIdeSettings.StartpageShowRecentsBlf)
                    {
                        btnRecent.Content = string.Format("Map Image - {0}", entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }
                    else if (entry.FileType == Settings.RecentFileType.MapInfo &&
                             App.ModernIdeStorage.ModernIdeSettings.StartpageShowRecentsMapInfo)
                    {
                        btnRecent.Content = string.Format("Map Info - {0}", entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }
                    else if (entry.FileType == Settings.RecentFileType.Campaign &&
                             App.ModernIdeStorage.ModernIdeSettings.StartpageShowRecentsCampaign)
                    {
                        btnRecent.Content = string.Format("Campaign - {0}", entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }

                    recentsCount++;
                }
            }
            else if (App.ModernIdeStorage.ModernIdeSettings.ApplicationRecents.Count == 0)
                panelRecents.Children.Add(new TextBlock
                {
                    Text = "It's lonely in here, get modding ;)",
                    Style = (Style) FindResource("GenericTextblock"),
                    Margin = new Thickness(20, 0, 0, 0)
                });
        }

        #region Settings Modification

        private void cbClosePageOnLoad_Update(object sender, RoutedEventArgs e)
        {
            if (cbClosePageOnLoad.IsChecked != null)
                App.ModernIdeStorage.ModernIdeSettings.StartpageHideOnLaunch = (bool) cbClosePageOnLoad.IsChecked;
        }

        private void cbShowOnStartUp_Update(object sender, RoutedEventArgs e)
        {
            if (cbShowOnStartUp.IsChecked != null)
                App.ModernIdeStorage.ModernIdeSettings.StartpageShowOnLoad = (bool) cbShowOnStartUp.IsChecked;
        }

        #endregion

        #region Open Types of Cache Files

        private void btnOpenCacheFile_Click(object sender, RoutedEventArgs e)
        {
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.OpenContentFile(Home.ContentTypes.Map);
        }

        #endregion
    }
}