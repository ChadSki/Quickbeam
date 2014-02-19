using System.Windows;
using ModernIde.Dialogs.ControlDialogs;

namespace ModernIde.Dialogs
{
    public static class MetroPostGeneratorViewer
    {
        /// <summary>
        ///     Show a Metro Post Generator Viewer
        /// </summary>
        /// <param name="bbcode">The generated BBCode of the post.</param>
        /// <param name="modAuthor">The author of the mod.</param>
        public static void Show(string bbcode, string modAuthor)
        {
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.ShowMask();
            var msgBox = new PostGeneratorViewer(bbcode, modAuthor)
            {
                Owner = App.ModernIdeStorage.ModernIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            msgBox.ShowDialog();
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.HideMask();
        }
    }
}