using System.Windows;
using MetroIde.Dialogs.ControlDialogs;

namespace MetroIde.Dialogs
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
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.ShowMask();
            var msgBox = new PostGeneratorViewer(bbcode, modAuthor)
            {
                Owner = App.MetroIdeStorage.MetroIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            msgBox.ShowDialog();
            App.MetroIdeStorage.MetroIdeSettings.HomeWindow.HideMask();
        }
    }
}