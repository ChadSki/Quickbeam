using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            InitializeComponent();

            cbClosePageOnLoad.IsChecked = App.MetroIdeStorage.MetroIdeSettings.StartpageHideOnLaunch;
            cbShowOnStartUp.IsChecked = App.MetroIdeStorage.MetroIdeSettings.StartpageShowOnLoad;
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
                        App.MetroIdeStorage.MetroIdeSettings.HomeWindow.AddCacheTabModule(senderEntry.FilePath);
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

                    if (entry.FileType == Settings.RecentFileType.Cache &&
                        App.MetroIdeStorage.MetroIdeSettings.StartpageShowRecentsMap)
                    {
                        btnRecent.Content = string.Format("{0} - {1}", entry.FileGame, entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }
                    else if (entry.FileType == Settings.RecentFileType.Blf &&
                             App.MetroIdeStorage.MetroIdeSettings.StartpageShowRecentsBlf)
                    {
                        btnRecent.Content = string.Format("Map Image - {0}", entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }
                    else if (entry.FileType == Settings.RecentFileType.MapInfo &&
                             App.MetroIdeStorage.MetroIdeSettings.StartpageShowRecentsMapInfo)
                    {
                        btnRecent.Content = string.Format("Map Info - {0}", entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }
                    else if (entry.FileType == Settings.RecentFileType.Campaign &&
                             App.MetroIdeStorage.MetroIdeSettings.StartpageShowRecentsCampaign)
                    {
                        btnRecent.Content = string.Format("Campaign - {0}", entry.FileName.Replace("_", "__"));
                        panelRecents.Children.Add(btnRecent);
                    }

                    recentsCount++;
                }
            }
            else if (App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Count == 0)
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
                App.MetroIdeStorage.MetroIdeSettings.StartpageHideOnLaunch = (bool) cbClosePageOnLoad.IsChecked;
        }

        private void cbShowOnStartUp_Update(object sender, RoutedEventArgs e)
        {
            if (cbShowOnStartUp.IsChecked != null)
                App.MetroIdeStorage.MetroIdeSettings.StartpageShowOnLoad = (bool) cbShowOnStartUp.IsChecked;
        }

        #endregion

        #region Open Types of Cache Files

        private void btnOpenCacheFile_Click(object sender, RoutedEventArgs e)
        {
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.OpenContentFile(Home.ContentTypes.Map);
        }

        #endregion
    }
}