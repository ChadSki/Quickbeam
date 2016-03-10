﻿using System.Windows;
using MetroIde.Dialogs.ControlDialogs;

namespace MetroIde.Dialogs
{
    public static class MetroMessageBoxCode
    {
        /// <summary>
        ///     Shows a metro message box containing code in it.
        /// </summary>
        /// <param name="title">The title of the Message Box</param>
        /// <param name="code">The code to display</param>
        public static void Show(string title, string code)
        {
            var msgBox = new MessageBoxCode(title, code)
            {
                Owner = App.MetroIdeStorage.MetroIdeSettings.HomeWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            msgBox.ShowDialog();
        }

        /// <summary>
        ///     Show a Metro Message Box
        /// </summary>
        /// <param name="message">The code to display</param>
        public static void Show(string message)
        {
            Show("Assembly - Message Box", message);
        }
    }
}