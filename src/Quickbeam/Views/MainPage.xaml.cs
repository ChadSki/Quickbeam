using Quickbeam.Interfaces;
using Quickbeam.Native;
using Quickbeam.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;

namespace Quickbeam.Views
{
    public partial class MainPage : IView
    {
        public MainPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            InitializeComponent();

            DataContext = ViewModel = new MainPageViewModel();
            AddSublPage();
        }

        public bool Close()
        {
            return true;
        }

        private void BtnLaunchHalo_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(HaloSettings.HaloExePath)) return;
            var haloDirectory = Path.GetDirectoryName(HaloSettings.HaloExePath);
            if (haloDirectory == null || !Directory.Exists(haloDirectory)) return;

            HorizontalGridSplitter.IsEnabled = false;
            VerticalGridSplitter.IsEnabled = false;
            HaloGrid.Children.Add(new HaloView());
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
