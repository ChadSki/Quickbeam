using System;
using System.Windows;
using ModernIde.Helpers;

namespace ModernIde.Dialogs
{
    public static class MetroException
    {
        /// <summary>
        ///     Because you are the only exception.
        /// </summary>
        /// <param name="ex">The exception to pass into the dialog.</param>
        public static void Show(Exception ex)
        {
            // Run it though the dictionary, see if it can be made more user-friendlyKK

            ex = ExceptionDictionary.GetFriendlyException(ex);

            if (App.ModernIdeStorage.ModernIdeSettings.HomeWindow != null)
                App.ModernIdeStorage.ModernIdeSettings.HomeWindow.ShowMask();

            ControlDialogs.Exception exceptionDialog = App.ModernIdeStorage.ModernIdeSettings.HomeWindow != null
                ? new ControlDialogs.Exception(ex)
                {
                    Owner = App.ModernIdeStorage.ModernIdeSettings.HomeWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                }
                : new ControlDialogs.Exception(ex);
            exceptionDialog.ShowDialog();

            if (App.ModernIdeStorage.ModernIdeSettings.HomeWindow != null)
                App.ModernIdeStorage.ModernIdeSettings.HomeWindow.HideMask();
        }
    }
}