using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.ViewModels
{
    internal class MenuViewModel
    {
        #region Private Fields

        private readonly MainViewModel mainViewModel;

        #endregion Private Fields

        #region Public Constructors

        public MenuViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;

            LoadCommand = new Command(Load);
            ExportCommand = new Command(Export);
            CommitCommand = new Command(Commit);
            SyncCommand = new Command(() => mainViewModel.Work.Execute(Sync));
        }

        #endregion Public Constructors

        #region Private Methods

        private async Task Sync()
        {
            var songInfo = await SongInfoDB.Instance.GetAllChangedInformation();
            await mainViewModel.Database.ImportSongInfo(songInfo);
            await SongInfoDB.Instance.CommitInformation();
        }

        #endregion Private Methods

        #region Public Properties

        public Command CommitCommand { get; }

        public Command ExportCommand { get; }

        public Command LoadCommand { get; }

        public Command SyncCommand { get; }

        #endregion Public Properties

        #region Public Methods

        public async Task Load(string path)
        {
            try
            {
                mainViewModel.LoadedPlaylist = await PlaylistContainerReader.Read(path);
                Settings.Default.LastLoadedPlaylistContainerFile = path;
                Settings.Default.Save();
            }
            catch
            {
                Settings.Default.LastLoadedPlaylistContainerFile = null;
                Settings.Default.Save();
                throw;
            }
        }

        public async void TryLoadLastFile()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.LastLoadedPlaylistContainerFile))
                return;

            try
            {
                await Load(Settings.Default.LastLoadedPlaylistContainerFile);
            }
            catch (FileNotFoundException) { }
        }

        #endregion Public Methods

        #region Private Methods

        private async void Commit()
        {
            await SongInfoDB.Instance.CommitInformation();
        }

        private async void Export()
        {
            var (result, path) = Dialogs.ShowExportSongInfo();
            if (!result)
                return;

            await SongInfoWriter.Write(path, await SongInfoDB.Instance.GetAllChangedInformation());
        }

        private async void Load()
        {
            var (result, path) = Dialogs.ShowLoadContainer();
            if (!result)
                return;

            await Load(path);
        }

        #endregion Private Methods
    }
}