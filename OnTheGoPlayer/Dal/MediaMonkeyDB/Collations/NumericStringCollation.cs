using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Collations
{
    [SQLiteFunction(Name = "NUMERICSTRING", FuncType = FunctionType.Collation)]
    internal class NumericStringCollation : SQLiteFunction
    {
        #region Public Methods

        public override int Compare(string param1, string param2)
        {
            int.TryParse(param1, out var val1);
            int.TryParse(param2, out var val2);
            return val1.CompareTo(val2);
        }

        #endregion Public Methods
    }
}