using System.Data.SQLite;
using System.Globalization;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Collations
{
    [SQLiteFunction(Name = "IUNICODE", FuncType = FunctionType.Collation)]
    internal class IUnicodeCollation : SQLiteFunction
    {
        #region Public Methods

        public override int Compare(string param1, string param2)
        {
            return string.Compare(param1, param2, true, CultureInfo.InvariantCulture);
        }

        #endregion Public Methods
    }
}