using System.Windows;
using ModernIde.Metro.Dialogs.ControlDialogs;

namespace ModernIde.Metro.Dialogs
{
    public static class MetroImgurUpload
    {
        public static void Show(string imageId)
        {
            App.AssemblyStorage.AssemblySettings.HomeWindow.ShowMask();
            var upload = new ImgurUpload(imageId)
            {
                Owner = App.AssemblyStorage.AssemblySettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            upload.ShowDialog();
            App.AssemblyStorage.AssemblySettings.HomeWindow.HideMask();
        }
    }
}