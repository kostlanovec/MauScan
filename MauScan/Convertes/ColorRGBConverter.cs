using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauScan.Data;

namespace MauScan.Convertes
{
	public class ColorRGBConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ColorRGB colorRGB)
			{
				return Color.FromRgb((int)(colorRGB.Red * 255), (int)(colorRGB.Green * 255), (int)(colorRGB.Blue * 255));
			}
			return Colors.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
