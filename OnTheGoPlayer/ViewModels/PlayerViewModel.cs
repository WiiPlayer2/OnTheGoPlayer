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

namespace OnTheGoPlayer.ViewModels
{
    internal class PlayerViewModel : INotifyPropertyChanged
    {
#pragma warning disable 67

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

#pragma warning restore 67

        #region Public Properties

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
                Player.Play(SelectedSong);
        }

        #endregion Public Methods

        #region Private Methods

        private void OnLoadedPlaylistChanged()
        {
            Player.SetPlaylistContainer(LoadedPlaylist);
        }

        #endregion Private Methods
    }
}