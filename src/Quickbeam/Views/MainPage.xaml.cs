﻿using Quickbeam.Interfaces;
using Quickbeam.Native;
using Quickbeam.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;

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

        public static void RemoveHaloPage()
        {
            var haloViews = Instance.HaloGrid.Children.OfType<HaloView>().ToList();
            foreach (var child in haloViews)
                Instance.HaloGrid.Children.Remove(child);
            Instance.HorizontalGridSplitter.IsEnabled = true;
            Instance.VerticalGridSplitter.IsEnabled = true;
        }

        public static void RemoveSublPage()
        {
            var sublViews = Instance.SublGrid.Children.OfType<SublView>().ToList();
            foreach (var child in sublViews)
                Instance.SublGrid.Children.Remove(child);
        }
    }
}
