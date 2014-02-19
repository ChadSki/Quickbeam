using System.Windows;
using ModernIde.Helpers.Net;
using ModernIde.Dialogs.ControlDialogs;

namespace ModernIde.Dialogs
{
    public static class MetroUpdateDialog
    {
        public static void Show(UpdateInfo info, bool available)
        {
            // ill up date u
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.ShowMask();
            var updater = new Updater(info, available)
            {
                Owner = App.ModernIdeStorage.ModernIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            updater.ShowDialog();
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.HideMask();
        }
    }
}