using Quickbeam.Native;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for HaloViewport.xaml
    /// </summary>
    public partial class HaloViewport : UserControl
    {
        public HaloViewport()
        {
            InitializeComponent();
        }

        private void BtnLaunchHalo_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(HaloSettings.HaloExePath)) return;
            var haloDirectory = System.IO.Path.GetDirectoryName(HaloSettings.HaloExePath);
            if (haloDirectory == null || !Directory.Exists(haloDirectory)) return;
            HaloGrid.Children.Add(new HaloView());
        }
    }
}
