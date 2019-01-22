using Microsoft.Win32;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Collations;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Queries;
using OnTheGoPlayer.Models;
using SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    public class MMDBPlaylistContainerExporter : IPlaylistContainerExporter
    {
        #region Public Constructors

        static MMDBPlaylistContainerExporter()
        {
            CollationsHelper.InitSQLiteFunctions();
        }

        #endregion Public Constructors

        #region Private Fields

        private System.Data.SQLite.SQLiteConnection connection;

        #endregion Private Fields

        #region Public Properties

        public bool IsOpen => connection != null;

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

            var newConnection = new System.Data.SQLite.SQLiteConnection($"Data Source={path};Version=3;Read Only=True;");
            newConnection.Open();
            newConnection.BindFunctions();
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

        private (Song Song, string FullPath) Convert(MMDBSong song, Dictionary<int, string> mediaMap)
        {
            var fullPath = GetPath(song, mediaMap[(int)song.IDMedia]);
            var retSong = new Song()
            {
                ID = (int)song.ID,
                FileFormat = Path.GetExtension(fullPath).TrimStart('.'),
                Title = song.SongTitle,
                Artist = song.Artist,
                Album = song.Album,
            };
            return (retSong, fullPath);
        }

        private (Song Song, string FullPath) FindSong(MMDBSong song, Dictionary<int, string> mediaMap)
        {
            if (mediaMap.ContainsKey((int)song.IDMedia))
                return Convert(song, mediaMap);

            //var media = await connection.FindWithQueryAsync<MMDBMedia>("SELECT * FROM Medias WHERE IDMedia=?;", song.IDMedia);
            var drives = DriveInfo.GetDrives();
            // TODO check media data first (DriveLetter, DriveLabel)
            foreach (var drive in drives)
            {
                var path = GetPath(song, drive.Name);
                if (File.Exists(path))
                {
                    mediaMap[(int)song.IDMedia] = drive.Name;
                    return Convert(song, mediaMap);
                }
            }

            throw new FileNotFoundException($"Couldn't find song #{song.ID}.");
        }

        private string GetPath(MMDBSong song, string driveName)
        {
            var driveBaseName = driveName.Substring(0, driveName.LastIndexOf(':'));
            return $"{driveBaseName}{song.SongPath}";
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

        #endregion Public Methods
    }
}