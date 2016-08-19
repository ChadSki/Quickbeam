using NimbusSharp;
using System.Linq;
using System.Windows;

namespace WpfTestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var map = Workbench.Instance.OpenMap();
            var x = map.Tags().First();

            //treeView.Root = ;
        }
    }
}
