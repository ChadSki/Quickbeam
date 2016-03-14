using PythonBinding;
using System.Windows.Controls;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for TagView.xaml
    /// </summary>
    public partial class TagView : UserControl
    {
        public TagView(HaloTagProxy haloStruct)
        {
            InitializeComponent();
            DataContext = haloStruct.Data;
        }
    }
}
