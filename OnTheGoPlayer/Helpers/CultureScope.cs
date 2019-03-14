using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    [Janitor.SkipWeaving]
    internal class CultureScope : IDisposable
    {
        #region Private Fields

        private readonly CultureInfo threadCulture;

        #endregion Private Fields

        #region Private Constructors

        private CultureScope(CultureInfo scopeCulture, CultureInfo threadCulture)
        {
            this.threadCulture = threadCulture;
            CultureInfo.CurrentCulture = scopeCulture;
        }

        #endregion Private Constructors

        #region Public Methods

        public static CultureScope Create(CultureInfo culture) => new CultureScope(culture, CultureInfo.CurrentCulture);

        public static CultureScope Create(string culture) => Create(CultureInfo.GetCultureInfo(culture));

        public void Dispose()
        {
            CultureInfo.CurrentCulture = threadCulture;
        }

        #endregion Public Methods
    }
}