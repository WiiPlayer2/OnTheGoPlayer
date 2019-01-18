using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion Public Methods
    }
}