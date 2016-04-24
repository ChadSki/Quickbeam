using System;
using PythonBinding;
using System.Windows;
using System.Windows.Controls;
using Quickbeam.ViewModels;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        private void OpenMapFile_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException("Need to create a file open dialog here");
        }

        private void OpenMapPC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Home.Instance.ViewModel.Status = "Reading Halo's memory...";
                PythonInterpreter.Instance.OpenMap(HaloMemory.PC);
                Home.Instance.ViewModel.Status = "Map opened";
            }
            catch (NullReferenceException)
            {
                Home.Instance.ViewModel.Status = "Could not read Halo's memory. Is it running?";
            }
        }

        private void OpenMapCE_Click(object sender, RoutedEventArgs e)
        {
            PythonInterpreter.Instance.OpenMap(HaloMemory.CE);
        }
    }
}
