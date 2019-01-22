using Fody;
using Microsoft.Win32;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Dal.MediaMonkeyCOM;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using OnTheGoPlayer.Helpers;
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
    internal class ExportViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly MainViewModel mainViewModel;

        #endregion Private Fields

        #region Public Constructors

        public ExportViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;

            Progress = new ProgressData();
            ReloadCommand = new Command(Reload, () => !Progress.IsWorking);
            ExportCommand = new Command<PlaylistMetaData>(Export, _ => !Progress.IsWorking);
            LoadCommand = new Command<PlaylistMetaData>(Load, _ => !Progress.IsWorking);

            Progress.PropertyChanged += (_, __) =>
            {
                ReloadCommand.Refresh();
                ExportCommand.Refresh(null);
                LoadCommand.Refresh(null);
            };
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Command<PlaylistMetaData> ExportCommand { get; }

        public IEnumerable<IPlaylistContainerExporter> Exporters { get; } = new IPlaylistContainerExporter[]
        {
            new MMDBPlaylistContainerExporter(),
            new MMComPlaylistContainerExporter(),
        };

        public Command<PlaylistMetaData> LoadCommand { get; }

        public IEnumerable<PlaylistMetaData> LoadedPlaylists { get; private set; }

        public ProgressData Progress { get; }

        public Command ReloadCommand { get; }

        public IPlaylistContainerExporter SelectedExporter { get; set; }

        #endregion Public Properties

        #region Private Methods

        [ConfigureAwait(true)]
        private async void Export(PlaylistMetaData metaData)
        {
            Progress.Start();
            try
            {
                Progress.Report((null, "Selecting export location..."));
                var (result, path) = Dialogs.ShowSaveContainer();
                if (!result)
                {
                    Progress.Error("Exporting canceled.");
                    return;
                }

                var container = await SelectedExporter.ExportPlaylist(metaData.ID, Progress);
                await PlaylistContainerWriter.Write(path, container, Progress);

                Progress.Stop();
            }
            catch (Exception e)
            {
                Progress.Error(e);
            }
        }

        [ConfigureAwait(true)]
        private async void Load(PlaylistMetaData metaData)
        {
            await Progress.Do(async () =>
            {
                mainViewModel.LoadedPlaylist = await SelectedExporter.ExportPlaylist(metaData.ID, Progress);
            }, false);
        }

        private void OnSelectedExporterChanged()
        {
            LoadedPlaylists = Enumerable.Empty<PlaylistMetaData>();
            Reload();
        }

        [ConfigureAwait(true)]
        private async void Reload()
        {
            Progress.Start();
            try
            {
                if (!SelectedExporter.IsOpen)
                {
                    Progress.Report((null, "Opening exporter..."));
                    if (!await SelectedExporter.TryOpen(Application.Current.MainWindow))
                    {
                        Progress.Error("Exporter wasn't opened.");
                        return;
                    }
                }

                LoadedPlaylists = (await SelectedExporter.ListPlaylists()).OrderBy(o => o.Title).ToList();

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