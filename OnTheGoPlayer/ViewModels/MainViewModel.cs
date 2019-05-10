using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Helpers;
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
            Work = new WorkExecuter();

            Work.PropertyChanged += Work_PropertyChanged;
        }

        private void Work_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MapPropertyChanged(e, PropertyChanged, nameof(Work.IsWorking), nameof(IsWorking));
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public IMediaDatabase Database { get; set; }

        public ExportViewModel ExportViewModel { get; }

        public bool IsWorking => Work.IsWorking;

        public IPlaylistContainer LoadedPlaylist { get; set; }

        public MenuViewModel MenuViewModel { get; }

        public PlayerViewModel PlayerViewModel { get; private set; } = new PlayerViewModel();

        public WorkExecuter Work { get; }

        #endregion Public Properties

        #region Private Methods

        private void OnLoadedPlaylistChanged()
        {
            PlayerViewModel.LoadedPlaylist = LoadedPlaylist;
        }

        #endregion Private Methods
    }
}