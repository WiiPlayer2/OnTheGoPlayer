using OnTheGoPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OnTheGoPlayer.Helpers
{
    internal class CollectionLoader<T> : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher;

        private readonly SemaphoreSlim loadSemaphore = new SemaphoreSlim(1, 1);

        private LoadTask currentLoadTask;

        #endregion Private Fields

        #region Public Constructors

        public CollectionLoader()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public ObservableCollection<T> Collection { get; } = new ObservableCollection<T>();

        public bool IsEnabled { get; set; }

        public bool IsLoading { get; private set; }

        public IEnumerable<T> Items { get; set; }

        #endregion Public Properties

        #region Private Methods

        private async void OnIsEnabledChanged()
        {
            var isEnabled = IsEnabled;
            await loadSemaphore.WaitAsync();
            if (currentLoadTask != null)
                await currentLoadTask.SetEnabled(isEnabled);
            loadSemaphore.Release();
        }

        private async void OnItemsChanged()
        {
            var items = Items;
            await loadSemaphore.WaitAsync();
            if (currentLoadTask != null)
                await currentLoadTask.Stop();
            IsLoading = true;
            await dispatcher.InvokeAsync(Collection.Clear);
            currentLoadTask = new LoadTask(this, items);
            await currentLoadTask.SetEnabled(IsEnabled);
            loadSemaphore.Release();
        }

        #endregion Private Methods

        #region Private Classes

        private class LoadTask
        {
            #region Private Fields

            private const int BATCH_SIZE = 25;

            private const int DELAY_LENGTH = 50;

            private readonly IEnumerator<T> enumerator;

            private readonly CollectionLoader<T> loader;

            private CancellationTokenSource cancellationTokenSource;

            private Task loopTask;

            #endregion Private Fields

            #region Public Constructors

            public LoadTask(CollectionLoader<T> collectionLoader, IEnumerable<T> items)
            {
                enumerator = items.GetEnumerator();
                loader = collectionLoader;
            }

            #endregion Public Constructors

            #region Public Methods

            public async Task SetEnabled(bool isEnabled)
            {
                if (isEnabled)
                    Start();
                else
                    await Stop();
            }

            public void Start()
            {
                if (loopTask != null)
                    return;

                cancellationTokenSource = new CancellationTokenSource();
                loopTask = Task.Run(() => Loop(cancellationTokenSource.Token));
            }

            public async Task Stop()
            {
                if (loopTask == null)
                    return;

                cancellationTokenSource.Cancel();
                try { await loopTask; }
                catch (OperationCanceledException) { }
                loopTask = null;
            }

            #endregion Public Methods

            #region Private Methods

            private async Task Loop(CancellationToken token)
            {
                token.ThrowIfCancellationRequested();

                var n = 0;
                while (enumerator.MoveNext())
                {
                    await loader.dispatcher.InvokeAsync(() => loader.Collection.Add(enumerator.Current));

                    n++;
                    if (n >= BATCH_SIZE)
                    {
                        await Task.Delay(DELAY_LENGTH, token);
                        n = 0;
                    }

                    token.ThrowIfCancellationRequested();
                }

                loader.dispatcher.Invoke(() => loader.IsLoading = false);
            }

            #endregion Private Methods
        }

        #endregion Private Classes
    }
}