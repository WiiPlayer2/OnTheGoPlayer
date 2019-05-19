using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OnTheGoPlayer.ViewModels
{
    internal class SearchViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly IMediaDatabase database;

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly MainViewModel mainViewModel;

        private IPlaylistContainer searchPlaylist;

        private Task searchTask;

        private CancellationTokenSource searchTaskCancellationTokenSource;

        #endregion Private Fields

        #region Public Constructors

        public SearchViewModel(MainViewModel mainViewModel, IMediaDatabase mediaDatabase)
        {
            this.mainViewModel = mainViewModel;
            database = mediaDatabase;

            LoadPlaylistCommand = new Command(LoadPlaylist, () => IsLoading || searchPlaylist != null);

            FilteredSongsLoader.PropertyChanged += FilteredSongsLoader_PropertyChanged;
            FilteredSongsLoader.IsEnabled = true;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public CollectionLoader<FullSongInfo> FilteredSongsLoader { get; } = new CollectionLoader<FullSongInfo>();

        public bool IsLoading => IsSearching || FilteredSongsLoader.IsLoading;

        public bool IsSearching => !searchTask?.IsCompleted ?? false;

        public Command LoadPlaylistCommand { get; }

        public string SearchText { get; set; }

        #endregion Public Properties

        #region Private Methods

        private void FilteredSongsLoader_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MapPropertyChanged(e, PropertyChanged, nameof(FilteredSongsLoader.IsLoading), nameof(IsLoading));
        }

        private void LoadPlaylist()
        {
            mainViewModel.LoadedPlaylist = searchPlaylist;
        }

        private async Task<IEnumerable<FullSongInfo>> Map(IEnumerable<Song> songInfos)
            => await Task.WhenAll(songInfos.Select(async o => new FullSongInfo { Song = o, SongInfo = await SongInfoDB.Instance.Get(o.ID) }));

        private void OnSearchTextChanged()
        {
            searchTaskCancellationTokenSource?.Cancel();
            var txt = SearchText;
            if (string.IsNullOrWhiteSpace(txt))
            {
                FilteredSongsLoader.Items = Enumerable.Empty<FullSongInfo>();
                return;
            }

            searchTaskCancellationTokenSource = new CancellationTokenSource();
            searchTask = Task.Run(async () =>
            {
                searchPlaylist = await database.Search(txt, searchTaskCancellationTokenSource.Token);
                return await Map(searchPlaylist.Playlist.Songs);
            })
                .ContinueWith(
                    task => dispatcher.Invoke(() =>
                    {
                        FilteredSongsLoader.Items = task.Result;
                        LoadPlaylistCommand.Refresh();
                    }),
                    searchTaskCancellationTokenSource.Token,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);

            this.InvokePropertyChanged(PropertyChanged, nameof(IsLoading));
            searchTask.ContinueWith(_ => dispatcher.Invoke(() => this.InvokePropertyChanged(PropertyChanged, nameof(IsLoading))));
        }

        #endregion Private Methods
    }
}