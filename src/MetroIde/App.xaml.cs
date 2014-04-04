using System;
using System.Collections.Generic;
using System.Windows;
using MetroIde.Dialogs;
using Microsoft.Shell;
using MetroIde.Helpers;

namespace MetroIde
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return MetroIdeStorage.MetroIdeSettings.HomeWindow == null ||
                   MetroIdeStorage.MetroIdeSettings.HomeWindow.ProcessCommandLineArgs(args);
        }

        #endregion

        public static Storage MetroIdeStorage;

        [STAThread]
        public static void Main()
        {
            if (!SingleInstance<App>.InitializeAsFirstInstance("RecivedCommand")) return;

            var application = new App();

            application.InitializeComponent();
            application.Run();

            // Allow single instance code to perform cleanup operations
            SingleInstance<App>.Cleanup();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Snazzy exception dialog
			Current.DispatcherUnhandledException += (o, args) =>
			{
				MetroException.Show(args.Exception);

				args.Handled = true;
			};

            // Create Assembly Storage
            MetroIdeStorage = new Storage();

            // Create jumplist
            JumpLists.UpdateJumplists();

            // Set closing method
            Current.Exit += (o, args) =>
            {
                // Update Settings with Window Width/Height
                MetroIdeStorage.MetroIdeSettings.ApplicationSizeMaximize =
                    (MetroIdeStorage.MetroIdeSettings.HomeWindow.WindowState == WindowState.Maximized);
                if (MetroIdeStorage.MetroIdeSettings.ApplicationSizeMaximize) return;

                MetroIdeStorage.MetroIdeSettings.ApplicationSizeWidth =
                    MetroIdeStorage.MetroIdeSettings.HomeWindow.Width;
                MetroIdeStorage.MetroIdeSettings.ApplicationSizeHeight =
                    MetroIdeStorage.MetroIdeSettings.HomeWindow.Height;
            };
        }
    }
}