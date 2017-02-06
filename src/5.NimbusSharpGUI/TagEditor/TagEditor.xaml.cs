using System.Windows.Controls;

namespace NimbusSharpGUI.TagEditor
{
    /// <summary>
    /// Interaction logic for TagEditor.xaml
    /// </summary>
    public partial class TagEditor : UserControl
    {
        public TagEditor(MapExplorer.HaloTagNode x)
        {
            InitializeComponent();
            treeView.Root = new HaloTagNode(x.Tag);
        }
    }
}
