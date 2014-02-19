using System.Windows;
using ModernIde.Helpers;
using MessageBox = ModernIde.Dialogs.ControlDialogs.MessageBox;
using MessageBoxOptions = ModernIde.Dialogs.ControlDialogs.MessageBoxOptions;

namespace ModernIde.Dialogs
{
    public static class MetroMessageBox
    {
        /// <summary>
        ///     The diffrent Button Combinations that Metro Message Box's support
        /// </summary>
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        /// <summary>
        ///     The Results a Metro MessageBox can return
        /// </summary>
        public enum MessageBoxResult
        {
            OK,
            Yes,
            No,
            Cancel
        }

        /// <summary>
        ///     Show a Metro Message Box
        /// </summary>
        /// <param name="title">The title of the Message Box</param>
        /// <param name="message">The message to be displayed in the Message Box</param>
        public static void Show(string title, string message)
        {
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.ShowMask();
            var msgBox = new ControlDialogs.MessageBox(title, message)
            {
                Owner = App.ModernIdeStorage.ModernIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            msgBox.ShowDialog();
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.HideMask();
        }

        /// <summary>
        ///     Show a Metro Message Box
        /// </summary>
        /// <param name="message">The message to be displayed in the Message Box</param>
        public static void Show(string message)
        {
            Show("Assembly - Message Box", message);
        }

        /// <summary>
        ///     Show a Metro Message Box
        /// </summary>
        /// <param name="title">The title of the Message Box</param>
        /// <param name="message">The message to be displayed in the Message Box</param>
        /// <param name="buttons">The buttons to show in the Message Box</param>
        /// <returns>The button the user clicked</returns>
        public static MessageBoxResult Show(string title, string message, MessageBoxButtons buttons)
        {
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.ShowMask();
            var msgBox = new ControlDialogs.MessageBoxOptions(title, message, buttons)
            {
                Owner = App.ModernIdeStorage.ModernIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            msgBox.ShowDialog();
            App.ModernIdeStorage.ModernIdeSettings.HomeWindow.HideMask();

            return TempStorage.MessageBoxButtonStorage;
        }
    }
}