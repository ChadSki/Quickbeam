using System.Windows;
using MetroIde.Helpers.Native;

namespace MetroIde.Dialogs.ControlDialogs
{
    /// <summary>
    ///     Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox
    {
        public MessageBox(string title, string message)
        {
            InitializeComponent();
            lblTitle.Text = title;
            lblSubInfo.Text = message;
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}