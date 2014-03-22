using System.Windows;
using MetroIde.Helpers;
using MetroIde.Helpers.Native;

namespace MetroIde.Dialogs.ControlDialogs
{
    /// <summary>
    ///     Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();
            DwmDropShadow.DropShadowToWindow(this);

            string version = VariousFunctions.GetApplicationVersion();
            lblTitle.Text = lblTitle.Text.Replace("{version}", version);
        }

        private void btnActionClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}