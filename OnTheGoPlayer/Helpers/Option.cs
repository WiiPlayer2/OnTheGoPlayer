using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    internal struct Option<T>
    {
        #region Private Fields

        private readonly T value;

        #endregion Private Fields

        #region Public Constructors

        public Option(T value)
        {
            IsSome = true;
            this.value = value;
        }

        #endregion Public Constructors

        #region Public Properties

        public static Option<T> None { get; } = new Option<T>();

        public bool IsNone => !IsSome;

        public bool IsSome { get; }

        #endregion Public Properties

        #region Public Methods

        public T GetValueOrDefault()
            => IsSome ? value : default;

        public T GetValueOrThrow()
            => IsSome ? value : throw new InvalidOperationException();

        public S Map<S>(Func<T, S> onSome, Func<S> onNone)
            => IsSome ? onSome(value) : onNone();

        public void Match(Action<T> onSome, Action onNone)
            => Map<object>(
                v => { onSome(v); return null; },
                () => { onNone(); return null; });

        #endregion Public Methods
    }
}