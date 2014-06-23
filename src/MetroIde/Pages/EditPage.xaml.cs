using System.Windows;
using System.Windows.Controls;
using MetroIde.Helpers;

namespace MetroIde.Pages
{
    /// <summary>
    /// Interaction logic for EditPage.xaml
    /// </summary>
    public partial class EditPage : UserControl
    {
        protected PythonEnvironment Env;
        public EditPage()
        {
            InitializeComponent();
            Env = PythonEnvironment.Instance;
            DataContext = Env.RootObservableStruct;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Env.Execute(TextBox.Text);
        }
    }
}
