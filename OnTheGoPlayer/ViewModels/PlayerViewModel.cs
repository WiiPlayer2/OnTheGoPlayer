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
        public Song SelectedSong { get; set; }

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
                Task.Run(() => Player.Play(SelectedSong));
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

        private void OnLoadedPlaylistChanged()
        {
            Player.SetPlaylistContainer(LoadedPlaylist);
        }

        private void PlayerControlViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MapProperty(e, nameof(PlayerControlViewModel.CurrentSong), nameof(CurrentSong));
        }

        #endregion Private Methods
    }
}