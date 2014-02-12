using System.Windows;
using ModernIde.Helpers.Net;
using ModernIde.Metro.Dialogs.ControlDialogs;

namespace ModernIde.Metro.Dialogs
{
    public static class MetroUpdateDialog
    {
        public static void Show(UpdateInfo info, bool available)
        {
            // ill up date u
            App.AssemblyStorage.AssemblySettings.HomeWindow.ShowMask();
            var updater = new Updater(info, available)
            {
                Owner = App.AssemblyStorage.AssemblySettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            updater.ShowDialog();
            App.AssemblyStorage.AssemblySettings.HomeWindow.HideMask();
        }
    }
}