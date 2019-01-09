using NullGuard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnTheGoPlayer.ViewModels
{
    internal class Command : ICommand
    {
        #region Private Fields

        private readonly Action action;

        private readonly Func<bool> canExecuteFunc;

        private bool canExecute;

        #endregion Private Fields

        #region Public Constructors

        public Command(Action action)
            : this(action, () => true) { }

        public Command(Action action, Func<bool> canExecute)
        {
            this.action = action;
            canExecuteFunc = canExecute;
            this.canExecute = canExecute();
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CanExecuteChanged;

        #endregion Public Events

        #region Public Methods

        public bool CanExecute([AllowNull]object parameter)
        {
            Refresh();
            return canExecute;
        }

        public void Execute([AllowNull]object parameter)
        {
            if (CanExecute(parameter))
            {
                action();
            }
        }

        public void Refresh()
        {
            var currentState = canExecuteFunc();
            if (currentState != canExecute)
            {
                canExecute = currentState;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Public Methods
    }
}