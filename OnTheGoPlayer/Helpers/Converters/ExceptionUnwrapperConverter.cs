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
    [ValueConversion(typeof(Exception), typeof(string))]
    internal class ExceptionUnwrapperConverter : ConverterMarkupExtension
    {
        #region Public Methods

        [return: AllowNull]
        public override object Convert([AllowNull]object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            if (!(value is Exception exception))
                return null;

            var exceptions = exception.GetAllExceptions();
            var messages = exceptions.Where(o => !(o is AggregateException)).Select(o => o.Message).Distinct();
            return string.Join("\n", messages);
        }

        public override object ConvertBack([AllowNull]object value, Type targetType, [AllowNull]object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}