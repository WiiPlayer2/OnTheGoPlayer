using MadMilkman.Ini;
using SongsDB;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Queries
{
    internal class Query
    {
        #region Private Fields

        private readonly List<QueryCondition> conditions = new List<QueryCondition>();

        private readonly List<QuerySortOrder> sortOrders = new List<QuerySortOrder>();

        #endregion Private Fields

        #region Public Methods

        public static Query FromIni(IniFile iniFile)
        {
            var query = new Query();
            var advCfg = QueryAdvancedConfig.FromIniSection(iniFile.Sections["Adv"]);
            for (var i = 1; i <= advCfg.ConditionsCount; i++)
            {
                query.AddCondition(QueryCondition.FromIniSection(iniFile.Sections[$"AdvCond{i}"]));
            }
            for (var i = 1; i <= advCfg.OrdersCount; i++)
            {
                query.AddSortOrder(QuerySortOrder.FromIniSection(iniFile.Sections[$"AdvSO{i}"]));
            }
            return query;
        }

        public void AddCondition(QueryCondition condition) => conditions.Add(condition);

        public void AddSortOrder(QuerySortOrder sortOrder) => sortOrders.Add(sortOrder);

        public async Task<IEnumerable<MMDBSong>> Execute(SQLiteConnection connection)
        {
            var whereStatement = GenerateWhereStatement();
            var orderStatement = GenerateOrderByStatement();
            var query = $"SELECT * FROM Songs {whereStatement} {orderStatement};";
            Debug.WriteLine(query);
            var filteredSongs = await connection.Query<MMDBSong>(query);
            return filteredSongs;
        }

        private string GenerateOrderByStatement()
        {
            if (sortOrders.Any())
                return $"ORDER BY {string.Join(", ", sortOrders.Select(o => o.GenerateOrder()))}";
            else
                return string.Empty;
        }

        private string GenerateWhereStatement()
        {
            if (conditions.Any())
                return $"WHERE {string.Join(" AND ", conditions.Select(o => o.GenerateClause()))}";
            else
                return string.Empty;
        }

        #endregion Public Methods
    }
}