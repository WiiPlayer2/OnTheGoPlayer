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

        public MainViewModel(IMediaDatabase mediaDatabase)
        {
            Database = mediaDatabase;

            MenuViewModel = new MenuViewModel(this);
            ExportViewModel = new ExportViewModel(this);
            WorkExecuterViewModel = new WorkExecuterViewModel(new WorkExecuter());
            SearchViewModel = new SearchViewModel(this, mediaDatabase);

            Work.PropertyChanged += Work_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public IMediaDatabase Database { get; }

        public ExportViewModel ExportViewModel { get; }

        public bool IsWorking => Work.IsWorking;

        public IPlaylistContainer LoadedPlaylist { get; set; }

        public MenuViewModel MenuViewModel { get; }

        public PlayerViewModel PlayerViewModel { get; } = new PlayerViewModel();

        public SearchViewModel SearchViewModel { get; }

        public WorkExecuter Work => WorkExecuterViewModel.WorkExecuter;

        public WorkExecuterViewModel WorkExecuterViewModel { get; }

        #endregion Public Properties

        #region Private Methods

        private void OnLoadedPlaylistChanged()
        {
            PlayerViewModel.LoadedPlaylist = LoadedPlaylist;
        }

        private void Work_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MapPropertyChanged(e, PropertyChanged, nameof(Work.IsWorking), nameof(IsWorking));
        }

        #endregion Private Methods
    }
}