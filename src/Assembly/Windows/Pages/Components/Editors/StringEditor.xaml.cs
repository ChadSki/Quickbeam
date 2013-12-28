using System.Windows.Controls;
using Blamite.Blam;

namespace Assembly.Windows.Pages.Components.Editors
{
    /// <summary>
    ///     Interaction logic for StringEditor.xaml
    /// </summary>
    public partial class StringEditor : UserControl
    {
        private ICacheFile _cache;

        public StringEditor(ICacheFile cache)
        {
            InitializeComponent();

            _cache = cache;
        }
    }
}