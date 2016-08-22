using System.Windows.Controls;

namespace Quickbeam.Views
{
    /// <summary>
    /// Interaction logic for TagEditor.xaml
    /// </summary>
    public partial class TagEditor : UserControl
    {
        public TagEditor(NimbusSharpGUI.MapExplorer.HaloTagNode x)
        {
            InitializeComponent();
            treeView.Root = new NimbusSharpGUI.TagEditor.HaloTagNode(x.Tag);
        }
    }
}
