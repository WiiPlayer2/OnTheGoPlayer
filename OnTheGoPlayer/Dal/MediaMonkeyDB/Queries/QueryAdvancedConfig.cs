using MadMilkman.Ini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Queries
{
    internal class QueryAdvancedConfig
    {
        #region Public Properties

        public int ConditionsCount { get; }

        public int OrdersCount { get; }

        #endregion Public Properties

        #region Private Classes

        private class IniAdvancedConfig
        {
            #region Public Properties

            public int ConditionsCount { get; set; }

            public int OrdersCount { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes

        #region Public Constructors

        public QueryAdvancedConfig(int conditionsCount, int ordersCount)
        {
            ConditionsCount = conditionsCount;
            OrdersCount = ordersCount;
        }

        #endregion Public Constructors

        #region Public Methods

        public static QueryAdvancedConfig FromIniSection(IniSection iniSection)
        {
            var cfg = iniSection.Deserialize<IniAdvancedConfig>();
            return new QueryAdvancedConfig(cfg.ConditionsCount, cfg.OrdersCount);
        }

        #endregion Public Methods
    }
}