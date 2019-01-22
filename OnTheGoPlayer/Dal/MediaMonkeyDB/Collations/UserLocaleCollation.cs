using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Collations
{
    [SQLiteFunction(Name = "USERLOCALE", FuncType = FunctionType.Collation)]
    internal class UserLocaleCollation : SQLiteFunction
    {
        #region Public Methods

        public override int Compare(string param1, string param2)
        {
            return string.Compare(param1, param2, true);
        }

        #endregion Public Methods
    }
}