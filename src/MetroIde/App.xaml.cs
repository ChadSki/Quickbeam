using System;
using System.Windows;
using MetroIde.Helpers;

namespace MetroIde
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static Storage MetroIdeStorage;

        [STAThread]
        public static void Main()
        {
            var application = new App();

            application.InitializeComponent();
            application.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //// Snazzy exception dialog
            //Current.DispatcherUnhandledException += (o, args) =>
            //{
            //    MetroException.Show(args.Exception);
            //    args.Handled = true;
            //};

            // CreateByteArray Assembly Storage
            MetroIdeStorage = new Storage();

            // CreateByteArray jumplist
            JumpLists.UpdateJumplists();

            // Initialize Python env
            PythonEnvironment.Initialize();

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