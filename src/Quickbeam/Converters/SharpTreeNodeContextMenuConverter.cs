using ICSharpCode.TreeView;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Quickbeam.Converters
{
    [ValueConversion(typeof(SharpTreeNode), typeof(ContextMenu))]
    public class SharpTreeNodeContextMenuConverter : IValueConverter
    {
        public ContextMenu TagContextMenu { get; set; }

        public ContextMenu FolderContextMenu { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as SharpTreeNode;
            if (item.Children.Count > 0)
                return FolderContextMenu;
            return TagContextMenu;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
