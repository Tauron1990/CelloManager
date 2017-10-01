using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AutoReleaser.Converter
{
    [ValueConversion(typeof(bool), typeof(ImageSource))]
    public sealed class BoolToImageConverter : IValueConverter
    {
        public ImageSource TrueSource { get; set; }
        public ImageSource FalseSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return FalseSource;

            return (bool) value ? TrueSource : FalseSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource source = value as ImageSource;

            return Equals(source, TrueSource);
        }
    }
}