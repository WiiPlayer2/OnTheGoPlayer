using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    public class WorkExecuter : INotifyPropertyChanged
    {
        #region Private Fields

        private int workCount = 0;

        #endregion Private Fields

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public bool IsWorking { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public Task Execute(Action action, Action<Exception> onException = null) => InternalExecute(() => Task.Run(action), onException);

        public Task Execute(Func<Task> asyncAction, Action<Exception> onException = null) => InternalExecute(() => Task.Run(asyncAction), onException);

        private async Task InternalExecute(Func<Task> asyncTaskCreator, Action<Exception> onException)
        {
            Interlocked.Increment(ref workCount);
            IsWorking = true;
            await asyncTaskCreator()
                .ContinueWith(task => onException?.Invoke(task.Exception), TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith(_ =>
                {
                    if (Interlocked.Decrement(ref workCount) == 0)
                        IsWorking = false;
                });
        }

        #endregion Public Methods
    }
}