using NullGuard;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OnTheGoPlayer.Helpers.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    internal class BooleanToVisibilityConverter : ConverterMarkupExtension
    {
        #region Public Properties

        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public Visibility TrueValue { get; set; } = Visibility.Visible;

        #endregion Public Properties

        #region Public Methods

        public override object Convert(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
            => value is bool booleanValue && booleanValue
                ? Visibility.Visible
                : Visibility.Collapsed;

        public override object ConvertBack(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}