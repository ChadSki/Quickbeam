using System.Windows.Controls;
using MetroIde.Helpers.Lobby;

namespace MetroIde.Pages
{
    /// <summary>
    /// Interaction logic for LobbyPage.xaml
    /// </summary>
    public partial class LobbyPage : UserControl
    {
        public LobbyPage()
        {
            InitializeComponent();
            var slvm = new ServerListViewModel();
            DataContext = slvm.Servers;
        }
    }
}
