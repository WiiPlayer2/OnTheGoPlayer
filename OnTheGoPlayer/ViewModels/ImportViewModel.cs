using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OnTheGoPlayer.ViewModels
{
    internal class ImportViewModel : INotifyPropertyChanged
    {
        #region Public Constructors

        public ImportViewModel()
        {
            ReloadCommand = new Command(Reload, () => !IsLoading);
        }

        #endregion Public Constructors

#pragma warning disable 67

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

#pragma warning restore 67

        #region Public Properties

        public IEnumerable<IPlaylistContainerExporter> Importers { get; } = new[]
        {
            new MMDBPlaylistContainerExporter(),
        };

        public bool IsLoading { get; private set; }

        public IEnumerable<(int ID, string Name)> Playlists { get; private set; }

        public Command ReloadCommand { get; }

        public IPlaylistContainerExporter SelectedImporter { get; set; }

        #endregion Public Properties

        #region Private Methods

        private void OnIsLoadingChanged() => ReloadCommand.Refresh();

        private void OnSelectedImporterChanged()
        {
            if (!SelectedImporter.IsOpen)
                Reload();
        }

        private async void Reload()
        {
            IsLoading = true;
            try
            {
                if (!SelectedImporter.IsOpen && !await SelectedImporter.TryOpen(Application.Current.MainWindow))
                    return;

                if (SelectedImporter.IsOpen)
                    Playlists = await SelectedImporter.ListPlaylists();
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion Private Methods
    }
}