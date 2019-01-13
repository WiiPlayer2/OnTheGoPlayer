using OnTheGoPlayer.Dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace OnTheGoPlayer.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region Public Constructors

        public MainViewModel()
        {
            MenuViewModel = new MenuViewModel(this);
            ExportViewModel = new ExportViewModel(this);
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public ExportViewModel ExportViewModel { get; }

        public IPlaylistContainer LoadedPlaylist { get; set; }

        public MenuViewModel MenuViewModel { get; }

        public PlayerViewModel PlayerViewModel { get; private set; } = new PlayerViewModel();

        #endregion Public Properties

        #region Private Methods

        private void OnLoadedPlaylistChanged()
        {
            PlayerViewModel.LoadedPlaylist = LoadedPlaylist;
        }

        #endregion Private Methods
    }
}