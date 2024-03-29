﻿using OnTheGoPlayer.Dal.MediaMonkeyDB.Collations;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Queries;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OnTheGoPlayer.Dal.MediaMonkeyDB;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    using Newtonsoft.Json.Linq;
    using OnTheGoPlayer.Helpers;
    using System.Threading;

    internal abstract class BaseDBMediaDatabase<TProfileData> : IMediaDatabase<TProfileData>
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

        public abstract Guid ID { get; }

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
                connection.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
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
            var mapSemaphore = new SemaphoreSlim(1, 1);
            var result = await Task.WhenAll(songs.Select(o => FindSong(o, mediaMap, mapSemaphore)));

            return new StreamPlaylistContainer(new PlaylistMetaData()
            {
                ID = id,
                Title = playlist.PlaylistName,
            }, result, GetStream);
        }

        public virtual Task ImportSongInfo(IEnumerable<SongInfo> songInfos) => CheckedImportSongInfo(songInfos);

        public async Task<IEnumerable<PlaylistMetaData>> ListPlaylists()
        {
            var dbPlaylists = await connection.Query<MMDBPlaylist>("SELECT IDPlaylist, PlaylistName FROM Playlists;");
            return dbPlaylists.Select(o => new PlaylistMetaData
            {
                ID = (int)o.IDPlaylist,
                Title = o.PlaylistName,
            }).ToList();
        }

        public abstract Task Open(TProfileData profileData);

        Task IMediaDatabase.Open(JToken profileData) => Open(profileData.ToObject<TProfileData>());

        public async Task<IPlaylistContainer> Search(string query, CancellationToken cancellationToken)
        {
            var dbSongs = await connection.Query<MMDBSong>($"SELECT * FROM Songs WHERE Artist LIKE '%{query}%' OR Album LIKE '%{query}%' OR SongTitle LIKE '%{query}%'");

            var mediaMap = new Dictionary<int, string>();
            var mapSemaphore = new SemaphoreSlim(1, 1);
            var songs = await Task.WhenAll(dbSongs.Select(o => FindSong(o, mediaMap, mapSemaphore)));

            return new StreamPlaylistContainer(new PlaylistMetaData()
            {
                ID = -1,
                Title = query,
            }, songs, GetStream);
        }

        public abstract Task<Option<Profile<TProfileData>>> TryRegister();

        async Task<Option<Profile>> IMediaDatabase.TryRegister() => (await TryRegister()).Map<Profile>(o => o);

        #endregion Public Methods

        #region Protected Methods

        protected async Task<bool> CheckedImportSongInfo(IEnumerable<SongInfo> songInfos)
        {
            if (!songInfos.Any())
                return false;

            foreach (var songInfo in songInfos)
            {
                var dbSong = (await connection.Find<MMDBSong>("SELECT PlayCounter, LastTimePlayed FROM Songs WHERE ID = ?;", songInfo.SongID)).ToOption();
                await dbSong.Match(async song =>
                {
                    var playCount = songInfo.PlayCount - songInfo.CommitedPlayCount + song.PlayCounter;
                    var lastPlayed = songInfo.LastPlayed > song.LastTimePlayedDateTime ? song.LastTimePlayedDateTime : songInfo.LastPlayed;

                    var cmd = connection.CreateCommand(
                        "UPDATE Songs SET PlayCounter = ?, LastTimePlayed = ? WHERE ID = ?;",
                        playCount,
                        lastPlayed.ToOption().Match<double?>(o => o.ToOADate(), () => null),
                        songInfo.SongID);
                    await cmd.ExecuteNonQueryAsync();
                }, () => Task.CompletedTask);
            }
            return true;
        }

        protected abstract Task<string> FindMap(MMDBSong song);

        protected abstract string GetPath(MMDBSong song, string mappedMediaName);

        protected abstract Task<Stream> GetStream(string path, CancellationToken cancellationToken);

        protected async Task OpenDatabase(string path)
        {
            await Close();

            var newConnection = new SQLiteConnection($"Data Source={path};Version=3;Read Only=False;");
            newConnection.Open();
            newConnection.BindFunctions();
            // TODO check schema
            connection = newConnection;
        }

        #endregion Protected Methods

        #region Private Methods

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

        private async Task<(Song Song, string FullPath)> FindSong(MMDBSong song, Dictionary<int, string> mediaMap, SemaphoreSlim mapSemaphore)
        {
            // TODO optimize
            await mapSemaphore.WaitAsync();
            try
            {
                if (mediaMap.ContainsKey((int)song.IDMedia))
                    return Convert(song, mediaMap);

                var map = await FindMap(song);
                mediaMap[(int)song.IDMedia] = map;
                return Convert(song, mediaMap);
            }
            finally
            {
                mapSemaphore.Release();
            }
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