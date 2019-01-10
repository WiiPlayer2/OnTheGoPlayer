using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OnTheGoPlayer.Helpers.Converters
{
    internal abstract class ConverterMarkupExtension : MarkupExtension, IValueConverter
    {
        #region Public Methods

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        #endregion Public Methods
    }
}