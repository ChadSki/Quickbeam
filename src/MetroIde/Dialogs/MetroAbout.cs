using System.Windows;
using MetroIde.Dialogs.ControlDialogs;

namespace MetroIde.Dialogs
{
    public static class MetroAbout
    {
        /// <summary>
        ///     Show the About Window
        /// </summary>
        public static void Show()
        {
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.ShowMask();
            var about = new About
            {
                Owner = App.MetroIdeStorage.MetroIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            about.ShowDialog();
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.HideMask();
        }
    }
}