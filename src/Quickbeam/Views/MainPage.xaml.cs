using Quickbeam.Interfaces;
using Quickbeam.ViewModels;

namespace Quickbeam.Views
{
    public partial class MainPage : IView
    {
        public static MainPage Instance
        {
            get { return HomeWindow.Instance.ViewModel.MainPage as MainPage; }
        }

        public MainPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainPageViewModel();
        }

        public bool Close()
        {
            return true;
        }
    }
}
