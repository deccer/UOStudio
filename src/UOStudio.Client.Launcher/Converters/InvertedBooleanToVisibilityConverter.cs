using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UOStudio.Client.Launcher.Converters
{
    public sealed class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = System.Convert.ToBoolean(value);
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}