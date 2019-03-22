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

        public ObservableCollection<T> Collection { get; private set; } = new ObservableCollection<T>();

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
            var collection = await dispatcher.InvokeAsync(() => new ObservableCollection<T>());
            currentLoadTask = new LoadTask(dispatcher, collection, items, () => IsLoading = false);
            await currentLoadTask.AddBatch();
            await dispatcher.InvokeAsync(() => Collection = collection);
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

            private readonly ObservableCollection<T> collection;

            private readonly Dispatcher dispatcher;

            private readonly IEnumerator<T> enumerator;

            private readonly Action loadingReset;

            private CancellationTokenSource cancellationTokenSource;

            private Task loopTask;

            #endregion Private Fields

            #region Public Constructors

            public LoadTask(Dispatcher dispatcher, ObservableCollection<T> collection, IEnumerable<T> items, Action loadingReset)
            {
                enumerator = items.GetEnumerator();
                this.dispatcher = dispatcher;
                this.collection = collection;
                this.loadingReset = loadingReset;
            }

            #endregion Public Constructors

            #region Public Methods

            public async Task<bool> AddBatch()
            {
                bool hasItem = false;
                for (var i = 0; i < BATCH_SIZE && (hasItem = enumerator.MoveNext()); i++)
                {
                    await dispatcher.InvokeAsync(() => collection.Add(enumerator.Current));
                }
                return hasItem;
            }

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

                while (await AddBatch())
                {
                    await Task.Delay(DELAY_LENGTH, token);
                    token.ThrowIfCancellationRequested();
                }

                dispatcher.Invoke(loadingReset);
            }

            #endregion Private Methods
        }

        #endregion Private Classes
    }
}