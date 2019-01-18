using NullGuard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnTheGoPlayer.ViewModels
{
    internal class Command<T> : ICommand
        where T : class
    {
        #region Private Fields

        private readonly Action<T> action;

        private readonly Func<T, bool> canExecuteFunc;

        private bool canExecute;

        #endregion Private Fields

        #region Public Constructors

        public Command(Action<T> action, bool async = false)
            : this(action, _ => true, async) { }

        public Command(Action<T> action, Func<T, bool> canExecute, bool async = false)
            : this(action, canExecute, null, async) { }

        public Command(Action<T> action, Func<T, bool> canExecute, INotifyPropertyChanged notifyingObject, bool async = false)
        {
            if (async)
                this.action = o => Task.Run(() => action(o));
            else
                this.action = action;

            canExecuteFunc = canExecute;
            this.canExecute = canExecute(null);

            if (notifyingObject != null)
                notifyingObject.PropertyChanged += (_, __) => Refresh(default(T));
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CanExecuteChanged;

        #endregion Public Events

        #region Public Methods

        public bool CanExecute([AllowNull]object parameter)
        {
            Refresh(parameter as T);
            return canExecute;
        }

        public void Execute([AllowNull]object parameter)
        {
            if (CanExecute(parameter))
            {
                action(parameter as T);
            }
        }

        public void Refresh([AllowNull]T parameter)
        {
            var currentState = canExecuteFunc(parameter);
            if (currentState != canExecute)
            {
                canExecute = currentState;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Public Methods
    }

    internal class Command : Command<object>
    {
        #region Public Constructors

        public Command(Action action, bool async = false)
            : this(action, () => true, async) { }

        public Command(Action action, Func<bool> canExecute, bool async = false)
            : this(action, canExecute, null, async) { }

        public Command(Action action, Func<bool> canExecute, INotifyPropertyChanged notifyingObject, bool async = false)
            : base(_ => action(), _ => canExecute(), notifyingObject, async) { }

        #endregion Public Constructors

        #region Public Methods

        public void Refresh() => Refresh(null);

        #endregion Public Methods
    }
}