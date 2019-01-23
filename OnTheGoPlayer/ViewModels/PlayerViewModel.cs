using NullGuard;
using OnTheGoPlayer.Bl;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OnTheGoPlayer.ViewModels
{
    internal class PlayerViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly SongInfoDB songInfoDB = new SongInfoDB();

        #endregion Private Fields

        #region Public Constructors

        public PlayerViewModel()
        {
            PlayerControlViewModel.PropertyChanged += PlayerControlViewModel_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Song CurrentSong => PlayerControlViewModel.CurrentSong;

        public IPlaylistContainer LoadedPlaylist { get; set; }

        public Player Player => PlayerControlViewModel.Player;

        public PlayerControlViewModel PlayerControlViewModel { get; } = new PlayerControlViewModel();

        [AllowNull]
        public FullSongInfo SelectedSong { get; set; }

        public IEnumerable<FullSongInfo> Songs { get; private set; }

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
            Songs = await Task.WhenAll(LoadedPlaylist.Playlist.Songs.Select(async o => new FullSongInfo() { Song = o, SongInfo = await songInfoDB.Get(o.ID) }));
        }

        private async void PlayerControlViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MapProperty(e, nameof(PlayerControlViewModel.CurrentSong), nameof(CurrentSong));

            if (e.PropertyName == nameof(PlayerControlViewModel.CurrentSong))
            {
                // TODO urrrg
                Songs = await Task.WhenAll(LoadedPlaylist.Playlist.Songs.Select(async o => new FullSongInfo() { Song = o, SongInfo = await songInfoDB.Get(o.ID) }));
            }
        }

        #endregion Private Methods
    }
}