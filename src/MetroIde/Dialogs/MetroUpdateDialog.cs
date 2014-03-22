using System.Windows;
using MetroIde.Helpers.Net;
using MetroIde.Dialogs.ControlDialogs;

namespace MetroIde.Dialogs
{
    public static class MetroUpdateDialog
    {
        public static void Show(UpdateInfo info, bool available)
        {
            // ill up date u
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.ShowMask();
            var updater = new Updater(info, available)
            {
                Owner = App.MetroIdeStorage.MetroIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            updater.ShowDialog();
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.HideMask();
        }
    }
}