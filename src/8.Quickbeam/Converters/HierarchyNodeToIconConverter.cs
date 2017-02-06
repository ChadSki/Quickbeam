using NimbusSharpGUI.MapExplorer;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quickbeam.Converters
{
    [ValueConversion(typeof(String), typeof(TemplateKey))]
    public class HierarchyNodeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is WorkbenchNode) ||
                (value is HaloMapNode) ||
                (value is HaloTagClassNode))
            {
                return Application.Current.FindResource("HierarchyClosedFolder");
            }
            return Application.Current.FindResource("HierarchyGenericTag");

            // return Application.Current.FindResource("HierarchySnd!Tag");
            // return Application.Current.FindResource("HierarchyBitmTag");
            // return Application.Current.FindResource("HierarchyModeTag");
            // return Application.Current.FindResource("HierarchyGenericTag");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
