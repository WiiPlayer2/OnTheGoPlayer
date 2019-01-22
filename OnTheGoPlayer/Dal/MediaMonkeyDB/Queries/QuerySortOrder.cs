using MadMilkman.Ini;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Queries
{
    internal class QuerySortOrder
    {
        #region Public Constructors

        public QuerySortOrder(string tableColumn, bool ascending)
        {
            TableColumn = tableColumn;
            Ascending = ascending;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Ascending { get; }

        public string TableColumn { get; }

        #endregion Public Properties

        #region Public Methods

        public static QuerySortOrder FromIniSection(IniSection iniSection)
        {
            var iniSortOrder = iniSection.Deserialize<IniSortOrder>();
            Debug.WriteLine($"{iniSortOrder}");
            return new QuerySortOrder(iniSortOrder.Order, iniSortOrder.Asc != 0);
        }

        public string GenerateOrder()
        {
            var column = TableColumn;
            if (TableColumn == "Songs.DateAdded")
                column = $"CAST({TableColumn} AS INTEGER)";
            return $"{column} {(Ascending ? "ASC" : "DESC")}";
        }

        #endregion Public Methods

        #region Private Classes

        private class IniSortOrder
        {
            #region Public Properties

            public int Asc { get; set; }

            public string Order { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}