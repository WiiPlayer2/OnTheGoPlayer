using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Collations
{
    internal static class CollationsHelper
    {
        #region Public Methods

        public static void InitSQLiteFunctions()
        {
            SQLiteFunction.RegisterFunction(typeof(IUnicodeCollation));
            SQLiteFunction.RegisterFunction(typeof(NumericStringCollation));
            SQLiteFunction.RegisterFunction(typeof(UserLocaleCollation));
        }

        #endregion Public Methods

        #region Public Methods

        public static void BindFunction(this SQLiteConnection connection, SQLiteFunction function)
        {
            var attr = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), false).FirstOrDefault() as SQLiteFunctionAttribute;
            connection.BindFunction(attr, function);
        }

        public static void BindFunctions(this SQLiteConnection connection)
        {
            connection.BindFunction(new IUnicodeCollation());
            connection.BindFunction(new NumericStringCollation());
            connection.BindFunction(new UserLocaleCollation());
        }

        #endregion Public Methods
    }
}