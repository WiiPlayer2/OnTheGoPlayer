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
    [ValueConversion(typeof(object), typeof(bool))]
    internal class EqualityCompareConverter : ConverterMarkupExtension
    {
        #region Public Properties

        public object CompareTo { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override object Convert(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            return Equals(value, CompareTo);
        }

        public override object ConvertBack(object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}