#if !DEBUG
using ModernIde.Dialogs;
#endif
using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Shell;
using ModernIde.Helpers;

namespace ModernIde
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return ModernIdeStorage.ModernIdeSettings.HomeWindow == null ||
                   ModernIdeStorage.ModernIdeSettings.HomeWindow.ProcessCommandLineArgs(args);
        }

        #endregion

        public static Storage ModernIdeStorage;

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

#if !DEBUG
			Current.DispatcherUnhandledException += (o, args) =>
			{
				MetroException.Show(args.Exception);

				args.Handled = true;
			};
#endif

            // Create Assembly Storage
            ModernIdeStorage = new Storage();

            // Create jumplist
            JumpLists.UpdateJumplists();

            // Try and delete all temp data
            VariousFunctions.EmptyUpdaterLocations();

            // Update File Defaults
            FileDefaults.UpdateFileDefaults();

            // Set closing method
            Current.Exit += (o, args) =>
            {
                // Update Settings with Window Width/Height
                ModernIdeStorage.ModernIdeSettings.ApplicationSizeMaximize =
                    (ModernIdeStorage.ModernIdeSettings.HomeWindow.WindowState == WindowState.Maximized);
                if (ModernIdeStorage.ModernIdeSettings.ApplicationSizeMaximize) return;

                ModernIdeStorage.ModernIdeSettings.ApplicationSizeWidth =
                    ModernIdeStorage.ModernIdeSettings.HomeWindow.Width;
                ModernIdeStorage.ModernIdeSettings.ApplicationSizeHeight =
                    ModernIdeStorage.ModernIdeSettings.HomeWindow.Height;
            };
        }
    }
}