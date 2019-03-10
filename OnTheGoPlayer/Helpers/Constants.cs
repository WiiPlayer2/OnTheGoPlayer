using OnTheGoPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    internal static class Constants
    {
        #region Public Fields

        public static readonly string DataFolder;

        #endregion Public Fields

        #region Public Constructors

        static Constants()
        {
            var assembly = Assembly.GetEntryAssembly();
            var company = assembly.GetValue((AssemblyCompanyAttribute o) => o.Company);
            var product = assembly.GetValue((AssemblyProductAttribute o) => o.Product);
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), company, product);
            Directory.CreateDirectory(DataFolder);
        }

        #endregion Public Constructors

        #region Public Methods

        public static string GetDataPath(string subPath) => Path.Combine(DataFolder, subPath);

        #endregion Public Methods
    }
}