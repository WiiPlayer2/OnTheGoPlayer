using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dropbox.Api;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal.MediaMonkeyDropboxDB
{
    internal class MMDropboxDBMediaDatabase : IMediaDatabase
    {
        #region Private Fields

        private MMDBPlaylistContainerExporter database;

        #endregion Private Fields

        #region Public Properties

        public bool IsOpen { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public async Task Close()
        {
            await database.Close();
            IsOpen = false;
            database = null;
        }

        public Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress) => database.ExportPlaylist(id, progress);

        public Task ImportSongInfo(IEnumerable<SongInfo> songInfos)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PlaylistMetaData>> ListPlaylists() => database.ListPlaylists();

        public Task Open(string data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryOpen(Window ownerWindow)
        {
            var authDialog = new DropboxAuthDialog();
            if (!authDialog.ShowDialog() ?? false)
                return Task.FromResult(false);

            var client = new DropboxClient(authDialog.Response.AccessToken);
            var selectDialog = new DropboxSelectDatabaseDialog(client);
            if (!selectDialog.ShowDialog() ?? false)
                return Task.FromResult(false);

            database = selectDialog.MediaDatabase;
            IsOpen = true;
            return Task.FromResult(true);
        }

        #endregion Public Methods
    }
}