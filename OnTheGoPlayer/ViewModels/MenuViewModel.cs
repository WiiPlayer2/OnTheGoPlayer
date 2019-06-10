using NetUpdater.Core;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.ViewModels
{
    internal class MenuViewModel
    {
        #region Private Fields

        private const string MANIFEST_NAME = "MANIFEST";

        private readonly CliInvoker cliInvoker = new CliInvoker("./updater/NetUpdater.Cli.dll", MANIFEST_NAME);

        private readonly MainViewModel mainViewModel;

        private readonly Updater updater;

        private readonly Uri updateUri = new Uri("https://apps.dark-link.info/deploy/OnTheGoPlayer/");

        #endregion Private Fields

        #region Public Constructors

        public MenuViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;

            LoadCommand = new Command(Load);
            ExportCommand = new Command(Export);
            CommitCommand = new Command(Commit);
            SyncCommand = new Command(() => mainViewModel.Work.Execute(Sync));
            UpdateCommand = new Command(() => mainViewModel.Work.Execute(Update));

            var applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#if DEBUG
            updater = new Updater(WebLocator.Instance, applicationPath, MicroElements.Functional.OptionNone.Default, "master");
#else
            updater = new Updater(WebLocator.Instance, applicationPath, MANIFEST_NAME, MicroElements.Functional.OptionNone.Default);
#endif
        }

        #endregion Public Constructors

        #region Public Properties

        public Command CommitCommand { get; }

        public Command ExportCommand { get; }

        public Command LoadCommand { get; }

        public Command SyncCommand { get; }

        public Command UpdateCommand { get; }

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

        private async Task Sync()
        {
            var songInfo = await SongInfoDB.Instance.GetAllChangedInformation();
            await mainViewModel.Database.ImportSongInfo(songInfo);
            await SongInfoDB.Instance.CommitInformation();
        }

        private async Task Update()
        {
            var checkResult = await updater.GetNewerVersion(updateUri);
            await checkResult.Match(
                versionDataOption => versionDataOption.Match(
                    async versionData =>
                    {
                        var result = MessageBox.Show($"Update to version {versionData.Version}-{versionData.Channel} found. Do you want to install it?", "Update found", MessageBoxButton.YesNo);
                        if (result != MessageBoxResult.Yes)
                            return;

                        await cliInvoker.Update(updateUri, "master");
                        await Application.Current.Dispatcher.InvokeAsync(() => Application.Current.Shutdown());
                    },
                    async () => MessageBox.Show("No update found.")),
                async e => MessageBox.Show(e.ToString(), "Error checking for updates."));
        }

        #endregion Private Methods
    }
}