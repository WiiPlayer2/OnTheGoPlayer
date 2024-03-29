﻿using NullGuard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    public struct Option<T>
    {
        #region Private Fields

        private readonly T value;

        #endregion Private Fields

        #region Private Constructors

        private Option(T value)
        {
            IsSome = true;
            this.value = value;
        }

        #endregion Private Constructors

        #region Public Properties

        public static Option<T> None { get; } = new Option<T>();

        public bool IsNone => !IsSome;

        public bool IsSome { get; }

        #endregion Public Properties

        #region Public Methods

        public static Option<T> FromObject([AllowNull]T obj)
        {
            if (obj != null)
                return new Option<T>(obj);
            return None;
        }

        public static implicit operator Option<T>(T value) => new Option<T>(value);

        [return: AllowNull]
        public T GetValueOrDefault()
                    => IsSome ? value : default;

        public T GetValueOrThrow()
            => IsSome ? value : throw new InvalidOperationException();

        public Option<S> Map<S>(Func<T, S> map)
            => IsSome ? new Option<S>(map(value)) : Option<S>.None;

        [return: AllowNull]
        public S Match<S>(Func<T, S> onSome, Func<S> onNone)
            => IsSome ? onSome(value) : onNone();

        public void Match(Action<T> onSome, Action onNone)
            => Match<object>(
                v => { onSome(v); return null; },
                () => { onNone(); return null; });

        #endregion Public Methods
    }
}