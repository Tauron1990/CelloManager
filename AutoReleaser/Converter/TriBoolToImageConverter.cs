using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AutoReleaser.Converter
{
    [ValueConversion(typeof(bool?), typeof(ImageSource))]
    public sealed class TriBoolToImageConverter : IValueConverter
    {
        public ImageSource TrueSource { get; set; }
        public ImageSource FalseSource { get; set; }
        public ImageSource NeutralSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? nullBool = value as bool?;

            switch (nullBool)
            {
                case null:
                    return NeutralSource;
                case true:
                    return TrueSource;
            }
            return FalseSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource source = value as ImageSource;

            if (Equals(source, NeutralSource)) return null;
            if (Equals(source, TrueSource)) return (bool?) true;

            return (bool?) false;
        }
    }
}