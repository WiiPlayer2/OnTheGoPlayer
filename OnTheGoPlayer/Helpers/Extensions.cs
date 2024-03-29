﻿using SongsDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    internal static class Extensions
    {
        #region Public Methods

        public static IEnumerable<Exception> GetAllExceptions(this Exception e)
        {
            if (e.InnerException == null)
                return e.Yield();

            if (e is AggregateException aggregateException)
                return e.Yield().Concat(aggregateException.InnerExceptions.SelectMany(GetAllExceptions));
            else
                return e.Yield().Concat(e.InnerException.Yield());
        }

        public static TValue GetValue<TAttribute, TValue>(this Assembly assembly, Func<TAttribute, TValue> getFunc, TValue defaultValue = default(TValue))
                    where TAttribute : Attribute
        {
            var attr = assembly.GetCustomAttribute<TAttribute>();
            return attr != null ? getFunc(attr) : defaultValue;
        }

        public static void InvokePropertyChanged(this INotifyPropertyChanged obj, PropertyChangedEventHandler propertyChanged, string propertyName)
        {
            propertyChanged?.Invoke(obj, new PropertyChangedEventArgs(propertyName));
        }

        public static IEnumerable<SDBSongData> Iterate(this ISDBSongIterator iter)
        {
            iter.Next();
            while (!iter.EOF)
            {
                yield return iter.Item;
                iter.Next();
            }
        }

        public static async Task<IDisposable> Lock(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new ActionOnDispose(() => semaphore.Release());
        }

        public static void MapPropertyChanged(this INotifyPropertyChanged obj, PropertyChangedEventArgs e, PropertyChangedEventHandler propertyChanged, string propertyName, params string[] mappedPropertyNames)
        {
            if (e.PropertyName == propertyName)
            {
                foreach (var name in mappedPropertyNames)
                    obj.InvokePropertyChanged(propertyChanged, name);
            }
        }

        public static Option<T> ToOption<T>(this T value) => Option<T>.FromObject(value);

        // TODO maybe rewrite this to just use Option<>.FromObject()
        public static Option<T> ToOption<T>(this T? value)
            where T : struct
            => value.HasValue ? Option<T>.FromObject(value.Value) : Option<T>.None;

        public static IEnumerable<T> Yield<T>(this T obj)
        {
            if (obj == null)
                return Enumerable.Empty<T>();
            return new[] { obj };
        }

        #endregion Public Methods

        #region Private Classes

        [Janitor.SkipWeaving]
        private class ActionOnDispose : IDisposable
        {
            #region Private Fields

            private readonly Action action;

            #endregion Private Fields

            #region Public Constructors

            public ActionOnDispose(Action action)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
            }

            #endregion Public Constructors

            #region Public Methods

            public void Dispose() => action();

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}