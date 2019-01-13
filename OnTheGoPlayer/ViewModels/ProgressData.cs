using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OnTheGoPlayer.ViewModels
{
    public class ProgressData : INotifyPropertyChanged, IProgress<(double? Progress, string Text)>
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        #endregion Private Fields

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Enums

        public enum State
        {
            None,

            Progressing,

            Indeterminate,

            Error,
        }

        #endregion Public Enums

        #region Public Properties

        public State CurrentState { get; private set; }

        public bool HasError => CurrentState == State.Error;

        public bool IsIndeterminate => CurrentState == State.Indeterminate;

        public bool IsWorking => CurrentState != State.None && CurrentState != State.Error;

        public double Progress { get; private set; }

        public string Text { get; set; } = string.Empty;

        #endregion Public Properties

        #region Public Methods

        public void Do(Action action)
        {
            Do(() =>
            {
                action();
                return Task.CompletedTask;
            }).Wait();
        }

        public async Task Do(Func<Task> action)
        {
            Start();
            try
            {
                await action();
                Stop();
            }
            catch (Exception e)
            {
                Error(e);
            }
        }

        public void Error(string text) => SetStateAndText(State.Error, text);

        public void Error(Exception exception) => Error($"{exception.GetType().Name}: {exception.Message}");

        public void Report((double? Progress, string Text) report)
        {
            dispatcher.InvokeAsync(() =>
            {
                CurrentState = report.Progress.HasValue ? State.Progressing : State.Indeterminate;
                Progress = report.Progress ?? 0;
                Text = report.Text;
            });
        }

        public void Start() => SetStateAndText(State.Indeterminate, string.Empty);

        public void Stop() => SetStateAndText(State.None, string.Empty);

        #endregion Public Methods

        #region Private Methods

        private void SetStateAndText(State state, string text)
        {
            dispatcher.Invoke(() =>
            {
                CurrentState = state;
                Text = text;
            });
        }

        #endregion Private Methods
    }
}