using NullGuard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OnTheGoPlayer.Helpers.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    internal class InvertBooleanConverter : ConverterMarkupExtension
    {
        #region Public Methods

        public override object Convert(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                return !((bool)value);
            }

            return true;
        }

        public override object ConvertBack(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);

        #endregion Public Methods
    }
}