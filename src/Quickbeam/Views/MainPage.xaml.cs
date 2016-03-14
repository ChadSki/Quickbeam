﻿using PythonBinding;
using Quickbeam.Interfaces;
using Quickbeam.Native;
using Quickbeam.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quickbeam.Views
{
    public partial class MainPage : IView
    {
        public static MainPage Instance { get; private set; }
        public MainPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            Instance = this;
            DataContext = ViewModel = new MainPageViewModel();
            var map = PythonInterpreter.Instance.Children[0] as HaloMapProxy;
            var newTab = new LayoutAnchorable
            {
                Title = "Tag Editor",
                Content = new TagView(map.getGhost())
            };
            DocumentManager.Children.Add(newTab);
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
        

        public static void RemoveHaloPage()
        {
            var haloViews = Instance.HaloGrid.Children.OfType<HaloView>().ToList();
            foreach (var child in haloViews)
                Instance.HaloGrid.Children.Remove(child);
            Instance.HorizontalGridSplitter.IsEnabled = true;
            Instance.VerticalGridSplitter.IsEnabled = true;
        }
    }
}
