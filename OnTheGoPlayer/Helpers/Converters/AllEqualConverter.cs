using NullGuard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers.Converters
{
    internal class AllEqualConverter : MultiConverterMarkupExtension
    {
        #region Public Methods

        public override object Convert(object[] values, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            if (values == null)
                return true;
            if (!values.Any())
                return true;

            var first = values.First();
            return values.All(o => o?.Equals(first) ?? false);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, [AllowNull]object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}