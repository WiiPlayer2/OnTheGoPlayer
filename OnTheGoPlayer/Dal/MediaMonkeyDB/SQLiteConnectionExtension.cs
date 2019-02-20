using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal static class SQLiteConnectionExtension
    {
        #region Private Fields

        private static Dictionary<Type, Dictionary<Type, Func<object, object>>> typeConverters = new Dictionary<Type, Dictionary<Type, Func<object, object>>>();

        #endregion Private Fields

        #region Public Constructors

        static SQLiteConnectionExtension()
        {
            Register<long, int>(v => (int)v);
            Register<long, bool>(v => v != 0);
        }

        #endregion Public Constructors

        #region Public Methods

        public static async Task<T> Find<T>(this SQLiteConnection connection, string query, params object[] args)
            where T : new()
        {
            var result = await connection.Query<T>(query, args);
            return result.FirstOrDefault();
        }

        public static async Task<(string[], IEnumerable<object[]>)> Query(this SQLiteConnection connection, string query, params object[] args)
        {
            var command = connection.CreateCommand(query, args);

            var ret = new List<object[]>();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var arr = new object[reader.FieldCount];
                reader.GetValues(arr);
                ret.Add(arr);
            }

            var names = Enumerable.Range(0, reader.FieldCount).Select(o => reader.GetName(o)).ToArray();

            return (names, ret);
        }

        public static async Task<IEnumerable<T>> Query<T>(this SQLiteConnection connection, string query, params object[] args)
            where T : new()
        {
            var (names, rows) = await connection.Query(query, args);

            return rows.Select(o => Convert<T>(names, o));
        }

        #endregion Public Methods

        #region Private Methods

        private static T Convert<T>(string[] names, object[] row)
            where T : new()
        {
            var props = typeof(T).GetProperties();
            var ret = new T();

            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var value = row[i];
                var valType = value?.GetType();
                var prop = props.FirstOrDefault(o => o.Name == name);
                if (prop != null)
                {
                    if (prop.PropertyType.IsAssignableFrom(value?.GetType()))
                    {
                        prop.SetValue(ret, value);
                    }
                    else if (typeConverters.ContainsKey(valType) && typeConverters[valType].ContainsKey(prop.PropertyType))
                    {
                        var converter = typeConverters[valType][prop.PropertyType];
                        prop.SetValue(ret, converter(value));
                    }
                    else if (value is DBNull)
                    {
                        prop.SetValue(ret, null);
                    }
                    else
                    {
                        Debug.Fail($"Cannot convert {valType} to {prop.PropertyType}.");
                    }
                }
            }

            return ret;
        }

        private static SQLiteCommand CreateCommand(this SQLiteConnection connection, string query, params object[] args)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            for (var i = 0; i < args.Length; i++)
            {
                command.Parameters.AddWithValue(null, args[i]);
            }
            return command;
        }

        private static void Register<TIn, TOut>(Func<TIn, TOut> convertFunc)
        {
            if (!typeConverters.ContainsKey(typeof(TIn)))
                typeConverters[typeof(TIn)] = new Dictionary<Type, Func<object, object>>();
            var subDict = typeConverters[typeof(TIn)];
            subDict[typeof(TOut)] = o => convertFunc((TIn)o);
        }

        #endregion Private Methods
    }
}