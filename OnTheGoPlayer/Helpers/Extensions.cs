using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace OnTheGoPlayer.Helpers
{
    internal static class Extensions
    {
        #region Public Methods

        public static TValue GetValue<TAttribute, TValue>(this Assembly assembly, Func<TAttribute, TValue> getFunc, TValue defaultValue = default(TValue))
            where TAttribute : Attribute
        {
            var attr = assembly.GetCustomAttribute<TAttribute>();
            return attr != null ? getFunc(attr) : defaultValue;
        }

        public static void InvokePropertyChanged(this INotifyPropertyChanged obj, PropertyChangedEventHandler propertyChanged, string propertyName)
        {
            propertyChanged?.Invoke(obj, new PropertyChangedEventArgs(propertyName));
        }

        public static Option<T> ToOption<T>(this T value) => new Option<T>(value);

        public static IEnumerable<T> Yield<T>(this T obj)
        {
            if (obj == null)
                return Enumerable.Empty<T>();
            return new[] { obj };
        }

        #endregion Public Methods
    }
}