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
        }

        #endregion Public Constructors

        #region Public Properties

        public Command LoadCommand { get; }

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