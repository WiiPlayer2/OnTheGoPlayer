using Fody;
using Microsoft.Win32;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Dal.MediaMonkeyCOM;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace OnTheGoPlayer.ViewModels
{
    internal class ImportViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        #endregion Private Fields

        #region Public Constructors

        public ImportViewModel()
        {
            Progress = new ProgressData();
            ReloadCommand = new Command(Reload, () => !Progress.IsWorking);
            ExportCommand = new Command<PlaylistMetaData>(Export, _ => !Progress.IsWorking);

            Progress.PropertyChanged += (_, __) =>
            {
                ReloadCommand.Refresh();
                ExportCommand.Refresh(null);
            };
        }

        #endregion Public Constructors

#pragma warning disable 67

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

#pragma warning restore 67

        #region Public Properties

        public Command<PlaylistMetaData> ExportCommand { get; }

        public IEnumerable<IPlaylistContainerExporter> Importers { get; } = new IPlaylistContainerExporter[]
        {
            new MMDBPlaylistContainerExporter(),
            new MMComPlaylistContainerExporter(),
        };

        public IEnumerable<PlaylistMetaData> Playlists { get; private set; }

        public ProgressData Progress { get; }

        public Command ReloadCommand { get; }

        public IPlaylistContainerExporter SelectedImporter { get; set; }

        #endregion Public Properties

        #region Private Methods

        [ConfigureAwait(true)]
        private async void Export(PlaylistMetaData obj)
        {
            Progress.Start();
            try
            {
                Progress.Report((null, "Selecting export location..."));
                var saveFileDialog = new SaveFileDialog()
                {
                    AddExtension = true,
                    CheckPathExists = true,
                    DefaultExt = "container",
                    Filter = "Playlist Container (*.container)|*.container",
                };
                var result = saveFileDialog.ShowDialog(Application.Current.MainWindow) ?? false;
                if (!result)
                {
                    Progress.Error("Exporting canceled.");
                    return;
                }

                var container = await SelectedImporter.ExportPlaylist(obj.ID, Progress);
                await PlaylistContainerWriter.Write(saveFileDialog.FileName, container, Progress);

                Progress.Stop();
            }
            catch (Exception e)
            {
                Progress.Error(e);
            }
        }

        private void OnIsLoadingChanged()
        {
            ReloadCommand.Refresh();
            ExportCommand.Refresh(null);
        }

        private void OnSelectedImporterChanged()
        {
            Playlists = Enumerable.Empty<PlaylistMetaData>();
            Reload();
        }

        private async void Reload()
        {
            Progress.Start();
            try
            {
                if (!SelectedImporter.IsOpen)
                {
                    Progress.Report((null, "Opening exporter..."));
                    if (!await SelectedImporter.TryOpen(Application.Current.MainWindow))
                    {
                        Progress.Error("Exporter wasn't opened.");
                        return;
                    }
                }

                Playlists = (await SelectedImporter.ListPlaylists()).OrderBy(o => o.Title).ToList();

                Progress.Stop();
            }
            catch (Exception e)
            {
                Progress.Error(e);
            }
        }

        #endregion Private Methods
    }
}