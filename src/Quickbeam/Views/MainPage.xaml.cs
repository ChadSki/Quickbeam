using NimbusSharpGUI.MapExplorer;
using Quickbeam.Interfaces;
using System.Linq;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quickbeam.Views
{
    public partial class MainPage : IView
    {
        public static MainPage Instance
        {
            get { return HomeWindow.Instance.ViewModel.MainPage as MainPage; }
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = WorkbenchNode.Instance;
        }

        public bool Close()
        {
            return true;
        }

        private void OpenTag_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var contextMenu = menuItem.Parent as ContextMenu;
            if (contextMenu == null) return;

            var node = contextMenu.DataContext as ExplorerNode;
            var tag = node as HaloTagNode;
            if (tag == null) return;
            var newTab = new LayoutAnchorable
            {
                Title = "Tag Editor",
                Content = new TextBlock { Text = tag.Name },
            };
            var documentPane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (documentPane != null)
                documentPane.Children.Add(newTab);
        }
    }
}
