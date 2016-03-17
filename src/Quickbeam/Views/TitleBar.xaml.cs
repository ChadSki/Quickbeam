using System;
using PythonBinding;
using System.Windows;
using System.Windows.Controls;

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
            throw new NotImplementedException("Need to create a dialog here");
        }

        private void OpenMapPC_Click(object sender, RoutedEventArgs e)
        {
            PythonInterpreter.Instance.OpenMap(HaloMemory.PC);
        }

        private void OpenMapCE_Click(object sender, RoutedEventArgs e)
        {
            PythonInterpreter.Instance.OpenMap(HaloMemory.CE);
        }
    }
}
