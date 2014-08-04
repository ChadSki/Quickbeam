using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Quickbeam.Models;

namespace Quickbeam.Converters
{
	[ValueConversion(typeof(String), typeof(TemplateKey))]
	public class CacheEditorNodeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var cacheEditor = (CacheEditorNode)value;

			switch (cacheEditor.Type)
			{
				case CacheEditorType.Model:
					return Application.Current.FindResource("HierarchySpecializedModeTag");

				case CacheEditorType.Sound:
					return Application.Current.FindResource("HierarchySpecializedSnd!Tag");

				default:
					return Application.Current.FindResource("HierarchyGenericTag");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
