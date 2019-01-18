using Microsoft.Win32;
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

        public async Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress)
        {
            progress.Report((null, "Preparing..."));
            var playlist = await connection.FindWithQueryAsync<MMDBPlaylist>("SELECT * FROM Playlists WHERE IDPlaylist=?;", id);
            if (playlist == null)
                throw new Exception($"Playlist with id {id} not found.");

            if (playlist.IsAutoPlaylist)
                throw new NotSupportedException($"Auto-Playlists are not supported yet.");

            //var playlistSongs = await connection.QueryAsync<MMDBPlaylistSong>("SELECT * FROM PlaylistSongs WHER IDPlaylist=?;", id);
            var mediaMap = new Dictionary<int, string>();
            progress.Report((null, "Getting songs..."));
            var songs = await connection.QueryAsync<MMDBSong>("SELECT Songs.* FROM PlaylistSongs LEFT JOIN Songs ON PlaylistSongs.IDSong = Songs.ID WHERE PlaylistSongs.IDPlaylist = ? ORDER BY PlaylistSongs.SongOrder;", id);
            //var resultTasks = songs.Select(o => FindSong(o, mediaMap));
            //var result = await Task.WhenAll(resultTasks);
            progress.Report((null, "Mapping songs..."));
            var result = songs.Select(o => FindSong(o, mediaMap)).ToList();

            return new FilesPlaylistContainer(new PlaylistMetaData()
            {
                ID = id,
                Title = playlist.PlaylistName,
            }, result);
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

        private (Song Song, string FullPath) Convert(MMDBSong song, Dictionary<int, string> mediaMap)
        {
            var fullPath = GetPath(song, mediaMap[song.IDMedia]);
            var retSong = new Song()
            {
                ID = song.ID,
                FileFormat = Path.GetExtension(fullPath).TrimStart('.'),
                Title = song.SongTitle,
                Artist = song.Artist,
                Album = song.Album,
            };
            return (retSong, fullPath);
        }

        private (Song Song, string FullPath) FindSong(MMDBSong song, Dictionary<int, string> mediaMap)
        {
            if (mediaMap.ContainsKey(song.IDMedia))
                return Convert(song, mediaMap);

            //var media = await connection.FindWithQueryAsync<MMDBMedia>("SELECT * FROM Medias WHERE IDMedia=?;", song.IDMedia);
            var drives = DriveInfo.GetDrives();
            // TODO check media data first (DriveLetter, DriveLabel)
            foreach (var drive in drives)
            {
                var path = GetPath(song, drive.Name);
                if (File.Exists(path))
                {
                    mediaMap[song.IDMedia] = drive.Name;
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

        #endregion Public Methods
    }
}