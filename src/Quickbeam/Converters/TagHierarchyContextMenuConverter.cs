using Quickbeam.Helpers;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Quickbeam.Converters
{
    [ValueConversion(typeof(ExplorerNode), typeof(ContextMenu))]
    public class TagHierarchyContextMenuConverter : IValueConverter
    {
        public ContextMenu TagContextMenu { get; set; }

        public ContextMenu FolderContextMenu { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ExplorerNode;
            if (item == null) return null;

            //return item.IsTag ? TagContextMenu : FolderContextMenu;
            return TagContextMenu;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
