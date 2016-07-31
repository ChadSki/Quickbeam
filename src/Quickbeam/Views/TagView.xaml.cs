using CrappyCppBinding;
using System.Windows.Controls;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for TagView.xaml
    /// </summary>
    public partial class TagView : UserControl
    {
        public TagView(HaloTagNode haloTag)
        {
            InitializeComponent();
            DataContext = haloTag.Data;
        }
    }
}
