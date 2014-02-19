using System.Windows;
using ModernIde.Dialogs.ControlDialogs;

namespace ModernIde.Dialogs
{
    public static class MetroAbout
    {
        /// <summary>
        ///     Show the About Window
        /// </summary>
        public static void Show()
        {
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.ShowMask();
            var about = new About
            {
                Owner = App.ModernIdeStorage.ModernIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            about.ShowDialog();
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.HideMask();
        }
    }
}