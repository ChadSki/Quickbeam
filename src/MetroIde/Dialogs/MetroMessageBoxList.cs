using System.Collections.Generic;
using System.Windows;
using MetroIde.Dialogs.ControlDialogs;

namespace MetroIde.Dialogs
{
    public static class MetroMessageBoxList
    {
        /// <summary>
        ///     Shows a metro message box containing a list of items in it.
        /// </summary>
        /// <param name="title">The title of the Message Box</param>
        /// <param name="message">The message to be displayed in the Message Box</param>
        /// <param name="items">The items to be displayed in the message box.</param>
        public static bool Show(string title, string message, IEnumerable<object> items)
        {
            var msgBox = new MessageBoxList(title, message, items)
            {
                Owner = App.MetroIdeStorage.MetroIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            msgBox.ShowDialog();
            return msgBox.DialogResult ?? false;
        }

        /// <summary>
        ///     Show a Metro Message Box
        /// </summary>
        /// <param name="message">The message to be displayed in the Message Box</param>
        /// <param name="items">The items to be displayed in the message box.</param>
        public static bool Show(string message, IEnumerable<object> items)
        {
            return Show("Assembly - Message Box", message, items);
        }
    }
}