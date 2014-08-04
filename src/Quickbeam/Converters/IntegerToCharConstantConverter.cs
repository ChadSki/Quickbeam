using System;
using System.Globalization;
using System.Windows.Data;
using Blamite.Util;

namespace Quickbeam.Converters
{
	[ValueConversion(typeof(int), typeof(string))]
	public class IntegerToCharConstantConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return CharConstant.ToString((int) value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
