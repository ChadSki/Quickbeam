using System.Windows;
using System.Windows.Controls;

namespace ModernIde.Controls.WPFTreeView
{
    public class MetroTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MetroTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is MetroTreeViewItem);
        }
    }
}