using System;
using System.Windows;
using MetroIde.Helpers;

namespace MetroIde.Dialogs
{
    public static class MetroException
    {
        /// <summary>
        ///     Because you are the only exception.
        /// </summary>
        /// <param name="ex">The exception to pass into the dialog.</param>
        public static void Show(Exception ex)
        {
            // Run it though the dictionary, see if it can be made more user-friendly

            if (App.MetroIdeStorage.MetroIdeSettings.HomeWindow != null)
                App.MetroIdeStorage.MetroIdeSettings.HomeWindow.ShowMask();

            ControlDialogs.Exception exceptionDialog = App.MetroIdeStorage.MetroIdeSettings.HomeWindow != null
                ? new ControlDialogs.Exception(ex)
                {
                    Owner = App.MetroIdeStorage.MetroIdeSettings.HomeWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                }
                : new ControlDialogs.Exception(ex);
            exceptionDialog.ShowDialog();

            if (App.MetroIdeStorage.MetroIdeSettings.HomeWindow != null)
                App.MetroIdeStorage.MetroIdeSettings.HomeWindow.HideMask();
        }
    }
}