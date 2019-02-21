using System;
using System.Collections.Generic;
using System.Globalization;
using MadMilkman.Ini;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB.Queries
{
    internal class QueryCondition
    {
        #region Private Fields

        private readonly static Dictionary<DataType, Func<string, object>> dataTypeConverters = new Dictionary<DataType, Func<string, object>>();

        #endregion Private Fields

        #region Public Constructors

        static QueryCondition()
        {
            dataTypeConverters[DataType.String] = value => value;

            dataTypeConverters[DataType.TDateTime] = value =>
            {
                var doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                return DateTime.FromOADate(doubleValue);
            };

            dataTypeConverters[DataType.Integer] = value => int.Parse(value);
        }

        public QueryCondition(string tableColumn, string value, ConditionType conditionType)
        {
            TableColumn = tableColumn;
            Value = value;
            (ValueType, ConditionGenerator) = GetConditionGenerator(conditionType);
        }

        #endregion Public Constructors

        #region Public Enums

        public enum ConditionType
        {
            DaysAgo = 401,

            DoesContain = 501,

            DoesNotContain = 502,
        }

        public enum DataType
        {
            TDateTime,

            Integer,

            String,
        }

        #endregion Public Enums

        #region Public Properties

        public Func<FormattableString> ConditionGenerator { get; }

        public string TableColumn { get; }

        public string Value { get; }

        public DataType ValueType { get; }

        #endregion Public Properties

        #region Public Methods

        public static QueryCondition FromIniSection(IniSection iniSection)
        {
            var iniCondition = iniSection.Deserialize<IniCondition>();
            return new QueryCondition(iniCondition.DBField, iniCondition.Value, (ConditionType)iniCondition.Condition);
        }

        public string GenerateClause()
        {
            return ConditionGenerator().ToString(CultureInfo.InvariantCulture);
        }

        public (DataType, Func<FormattableString>) GetConditionGenerator(ConditionType conditionType)
        {
            switch (conditionType)
            {
                case ConditionType.DaysAgo:
                    return (DataType.Integer, () => $"{TableColumn} >= {DateTime.Now.AddDays(GetValue<int>() * -1).ToOADate()}");

                case ConditionType.DoesContain:
                    return (DataType.String, () => $"{TableColumn} like '%{GetValue<string>()}%'");

                case ConditionType.DoesNotContain:
                    return (DataType.String, () => $"NOT ({TableColumn} like '%{GetValue<string>()}%')");

                default:
                    throw new NotImplementedException($"Condition type {conditionType} is not implemented yet.");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private T GetValue<T>() => (T)dataTypeConverters[ValueType](Value);

        #endregion Private Methods

        #region Private Classes

        private class IniCondition
        {
            #region Public Properties

            public int Condition { get; set; }

            public string DBField { get; set; }

            public int DBFieldPerType { get; set; }

            public string Value { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}