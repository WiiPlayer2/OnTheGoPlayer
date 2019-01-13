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
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    internal class TimespanToDoubleConverter : ConverterMarkupExtension
    {
        #region Public Methods

        public override object Convert(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            return value is TimeSpan duration ? duration.TotalSeconds : (object)null;
        }

        public override object ConvertBack(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            return value is double seconds ? TimeSpan.FromSeconds(seconds) : (object)null;
        }

        #endregion Public Methods
    }
}