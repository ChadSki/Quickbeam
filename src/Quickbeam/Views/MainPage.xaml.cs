using Quickbeam.Interfaces;
using Quickbeam.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class MainPage : IView
    {
        public ReplPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            InitializeComponent();

            DataContext = ViewModel = new ReplPageViewModel();
            AddSublPage();
        }

        public bool Close()
        {
            return true;
        }

        private void BtnLaunchHalo_Click(object sender, RoutedEventArgs e)
        {
            var haloExePath = App.Storage.Settings.HaloExePath;
            if (File.Exists(haloExePath))
            {
                var haloDirectory = Path.GetDirectoryName(haloExePath);
                if (haloDirectory != null && Directory.Exists(haloDirectory))
                {
                    HorizontalGridSplitter.IsEnabled = false;
                    VerticalGridSplitter.IsEnabled = false;
                    HaloGrid.Children.Add(new HaloView());
                    return;
                }
            }
        }

        public void AddSublPage()
        {
            SublGrid.Children.Add(new SublView());
        }

        public void RemoveHaloPage()
        {
            var haloViews = HaloGrid.Children.OfType<HaloView>().ToList();
            foreach (var child in haloViews)
                HaloGrid.Children.Remove(child);
            HorizontalGridSplitter.IsEnabled = true;
            VerticalGridSplitter.IsEnabled = true;
        }

        public void RemoveSublPage()
        {
            var sublViews = SublGrid.Children.OfType<SublView>().ToList();
            foreach (var child in sublViews)
                SublGrid.Children.Remove(child);
        }
    }
}
