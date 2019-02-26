using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OnTheGoPlayer.Helpers
{
    internal static class Extensions
    {
        #region Public Methods

        public static void InvokePropertyChanged()
        {
        }

        public static IEnumerable<T> Yield<T>(this T obj)
        {
            if (obj == null)
                return Enumerable.Empty<T>();
            return new[] { obj };
        }

        public static TValue GetValue<TAttribute, TValue>(this Assembly assembly, Func<TAttribute, TValue> getFunc, TValue defaultValue = default(TValue))
            where TAttribute : Attribute
        {
            var attr = assembly.GetCustomAttribute<TAttribute>();
            return attr != null ? getFunc(attr) : defaultValue;
        }

        #endregion Public Methods
    }
}