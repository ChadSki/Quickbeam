using System.IO;
using System.Windows;
using System.Windows.Controls;
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
    }
}