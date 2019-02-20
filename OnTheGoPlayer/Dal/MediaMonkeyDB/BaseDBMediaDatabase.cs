using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Collations;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Queries;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal abstract class BaseDBMediaDatabase : IMediaDatabase
    {
        #region Private Fields

        private SQLiteConnection connection;

        #endregion Private Fields

        #region Public Constructors

        static BaseDBMediaDatabase()
        {
            CollationsHelper.InitSQLiteFunctions();
        }

        #endregion Public Constructors

        #region Public Properties

        public bool IsOpen => throw new NotImplementedException();

        #endregion Public Properties

        #region Public Methods

        public Task Close()
        {
            if (!IsOpen)
                return Task.CompletedTask;

            try
            {
                connection.Close();
            }
            // NullReferenceException is thrown when no connection has been opened yet.
            catch (NullReferenceException) { }
            finally
            {
                connection = null;
            }
            return Task.CompletedTask;
        }

        public async Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress)
        {
            progress.Report((null, "Preparing..."));
            var playlist = await connection.Find<MMDBPlaylist>("SELECT * FROM Playlists WHERE IDPlaylist=?;", id);
            if (playlist == null)
                throw new Exception($"Playlist with id {id} not found.");

            progress.Report((null, "Getting songs..."));
            IEnumerable<MMDBSong> songs;
            if (playlist.IsAutoPlaylist)
                songs = await QueryPlaylistSongs(playlist);
            else
                songs = await GetPlaylistSongs(id);

            progress.Report((null, "Mapping songs..."));
            var mediaMap = new Dictionary<int, string>();
            var result = songs.Select(o => FindSong(o, mediaMap)).ToList();

            return new FilesPlaylistContainer(new PlaylistMetaData()
            {
                ID = id,
                Title = playlist.PlaylistName,
            }, result);
        }

        public Task ImportSongInfo(IEnumerable<SongInfo> songInfos)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PlaylistMetaData>> ListPlaylists()
        {
            var dbPlaylists = await connection.Query<MMDBPlaylist>("SELECT IDPlaylist, PlaylistName FROM Playlists;");
            return dbPlaylists.Select(o => new PlaylistMetaData
            {
                ID = (int)o.IDPlaylist,
                Title = o.PlaylistName,
            }).ToList();
        }

        public async Task Open(string path)
        {
            await Close();

            var newConnection = new SQLiteConnection($"Data Source={path};Version=3;Read Only=True;");
            newConnection.Open();
            newConnection.BindFunctions();
            // TODO check schema
            connection = newConnection;
        }

        public abstract Task<bool> TryOpen(Window ownerWindow);

        #endregion Public Methods

        #region Private Methods

        private (Song Song, string FullPath) FindSong(MMDBSong song, Dictionary<int, string> mediaMap)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<MMDBSong>> GetPlaylistSongs(int playlistId)
        {
            return await connection.Query<MMDBSong>(
               "SELECT Songs.* FROM PlaylistSongs " +
               "LEFT JOIN Songs ON PlaylistSongs.IDSong = Songs.ID " +
               "WHERE PlaylistSongs.IDPlaylist = ? " +
               "ORDER BY PlaylistSongs.SongOrder;",
               playlistId);
        }

        private async Task<IEnumerable<MMDBSong>> QueryPlaylistSongs(MMDBPlaylist playlist)
        {
            var ini = new MadMilkman.Ini.IniFile();
            using (var reader = new StringReader(playlist.QueryData))
            {
                ini.Load(reader);
            }

            var query = Query.FromIni(ini);
            return await query.Execute(connection);
        }

        #endregion Private Methods
    }
}