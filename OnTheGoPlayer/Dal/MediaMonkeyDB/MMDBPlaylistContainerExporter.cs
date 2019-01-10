using Microsoft.Win32;
using OnTheGoPlayer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    public class MMDBPlaylistContainerExporter : IPlaylistContainerExporter
    {
        #region Private Fields

        private SQLiteAsyncConnection connection;

        #endregion Private Fields

        #region Public Properties

        public bool IsOpen => connection != null;

        #endregion Public Properties

        #region Public Methods

        public async Task Close()
        {
            if (!IsOpen)
                return;

            try
            {
                await connection.CloseAsync();
            }
            // NullReferenceException is thrown when no connection has been opened yet.
            catch (NullReferenceException) { }
            finally
            {
                connection = null;
            }
        }

        public Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PlaylistMetaData>> ListPlaylists()
        {
            var dbPlaylists = await connection.QueryAsync<MMDBPlaylist>("SELECT IDPlaylist, PlaylistName FROM Playlists;");
            return dbPlaylists.Select(o => new PlaylistMetaData
            {
                ID = o.IDPlaylist,
                Title = o.PlaylistName,
            }).ToList();
        }

        public async Task Open(string path)
        {
            await Close();

            var newConnection = new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadOnly);
            // TODO check schema
            connection = newConnection;
        }

        public async Task<bool> TryOpen(Window ownerWindow)
        {
            var openFileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "db",
                Filter = "MediaMonkey Database Files (*.db)|*.db",
                Multiselect = false,
            };
            var result = openFileDialog.ShowDialog(ownerWindow) ?? false;

            if (result)
                await Open(openFileDialog.FileName);

            return result;
        }

        #endregion Public Methods
    }
}