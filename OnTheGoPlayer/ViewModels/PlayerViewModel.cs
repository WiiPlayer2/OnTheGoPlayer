using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using NullGuard;
using OnTheGoPlayer.Bl;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.ViewModels
{
    internal class PlayerViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private Task filterTask;

        private CancellationTokenSource filterTaskCancellationTokenSource;

        private FullSongInfo lastSongInfo = null;

        private IEnumerable<FullSongInfo> songs;

        #endregion Private Fields

        #region Public Constructors

        public PlayerViewModel()
        {
            PlayerControlViewModel.PropertyChanged += PlayerControlViewModel_PropertyChanged;
            FilteredSongsLoader.PropertyChanged += FilteredSongsLoader_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Song CurrentSong => PlayerControlViewModel.CurrentSong;

        public CollectionLoader<FullSongInfo> FilteredSongsLoader { get; } = new CollectionLoader<FullSongInfo>();

        //public IEnumerable<FullSongInfo> FilteredSongs { get; private set; }
        public bool IsFiltering => filterTask != null && !filterTask.IsCompleted;

        public bool IsLoading => IsFiltering || FilteredSongsLoader.IsLoading;

        public IPlaylistContainer LoadedPlaylist { get; set; }

        public Player Player => PlayerControlViewModel.Player;

        public PlayerControlViewModel PlayerControlViewModel { get; } = new PlayerControlViewModel();

        public string SearchText { get; set; }

        [AllowNull]
        public FullSongInfo SelectedSong { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void DisposePlayer()
        {
            Player.Dispose();
            //Player = null;
        }

        public void PlaySelectedSong()
        {
            if (SelectedSong != null)
                Task.Run(() => Player.Play(SelectedSong.Song));
        }

        #endregion Public Methods

        #region Private Methods

        private void FilteredSongsLoader_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MapProperty(e, nameof(CollectionLoader<FullSongInfo>.IsLoading), nameof(IsLoading));
        }

        private void InvokePropertyChanged(string name)
        {
            //Debug.WriteLine($"InvokePropertyChanged({name})");
            dispatcher.InvokeAsync(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        private void MapProperty(PropertyChangedEventArgs eventArgs, string sourceProp, string targetProp)
        {
            if (eventArgs.PropertyName == sourceProp)
                InvokePropertyChanged(targetProp);
        }

        private async void OnLoadedPlaylistChanged()
        {
            Player.SetPlaylistContainer(LoadedPlaylist);
            songs = await Task.WhenAll(LoadedPlaylist.Playlist.Songs.Select(async o => new FullSongInfo() { Song = o, SongInfo = await SongInfoDB.Instance.Get(o.ID) }));
            FilteredSongsLoader.Items = songs;
        }

        private void OnSearchTextChanged()
        {
            filterTaskCancellationTokenSource?.Cancel();
            var txt = SearchText.ToLower();
            if (string.IsNullOrWhiteSpace(txt))
            {
                FilteredSongsLoader.Items = songs;
                return;
            }

            filterTaskCancellationTokenSource = new CancellationTokenSource();
            filterTask = Task.Run(() =>
                {
                    var filteredSongs = songs.AsParallel().Where(o =>
                            o.Song.Title.ToLower().Contains(txt) || o.Song.Artist.ToLower().Contains(txt) ||
                            o.Song.Album.ToLower().Contains(txt))
                        .WithCancellation(filterTaskCancellationTokenSource.Token)
                        .ToList();
                    filterTaskCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    return filteredSongs;
                }, filterTaskCancellationTokenSource.Token)
                .ContinueWith(task => dispatcher.Invoke(() => FilteredSongsLoader.Items = task.Result),
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            InvokePropertyChanged(nameof(IsFiltering));
            filterTask.ContinueWith(_ => dispatcher.Invoke(() => InvokePropertyChanged(nameof(IsFiltering))));
        }

        private async void PlayerControlViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MapProperty(e, nameof(PlayerControlViewModel.CurrentSong), nameof(CurrentSong));

            if (e.PropertyName == nameof(PlayerControlViewModel.CurrentSong))
            {
                if (lastSongInfo != null)
                {
                    lastSongInfo.SongInfo = await SongInfoDB.Instance.Get(lastSongInfo.Song.ID);
                }

                lastSongInfo = CurrentSong != null ? songs.Single(o => o.Song == CurrentSong) : null;
            }
        }

        #endregion Private Methods
    }
}