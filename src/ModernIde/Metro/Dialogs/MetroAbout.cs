using System.Windows;
using ModernIde.Metro.Dialogs.ControlDialogs;

namespace ModernIde.Metro.Dialogs
{
    public static class MetroAbout
    {
        /// <summary>
        ///     Show the About Window
        /// </summary>
        public static void Show()
        {
            App.AssemblyStorage.AssemblySettings.HomeWindow.ShowMask();
            var about = new About
            {
                Owner = App.AssemblyStorage.AssemblySettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            about.ShowDialog();
            App.AssemblyStorage.AssemblySettings.HomeWindow.HideMask();
        }
    }
}