using Newtonsoft.Json.Linq;
using NullGuard;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using SongsDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyCOM
{
    internal class MMComPlaylistContainerExporter : IMediaDatabase
    {
        #region Private Fields

        private SDBApplication application;

        #endregion Private Fields

        #region Public Properties

        public Guid ID => new Guid("8cbbf702-c526-4bb7-9dce-5533c4004a36");

        public bool IsOpen => application != null;

        #endregion Public Properties

        #region Public Methods

        public Task Close()
        {
            application = null;
            return Task.CompletedTask;
        }

        public Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress)
        {
            progress.Report((null, $"Exporting playlist #{id}..."));
            return Task.Run<IPlaylistContainer>(() =>
            {
                var sdbPlaylist = application.PlaylistByID[id];
                var metaData = ToPlaylistMetaData(sdbPlaylist);
                var tracks = sdbPlaylist.Tracks;
                var songs = Enumerable.Range(0, tracks.Count).Select(i =>
                {
                    progress.Report((((double)i) / tracks.Count, $"Reading song #{i + 1}/{tracks.Count}..."));
                    var data = ToSongData(tracks.Item[i]);
                    return data;
                }).ToList();
                progress.Report((1, "Finalizing container..."));

                return new FilesPlaylistContainer(metaData, songs);
            });
        }

        public Task ImportSongInfo(IEnumerable<SongInfo> songInfos)
        {
            var songList = application.NewSongList;
            foreach (var songInfo in songInfos)
            {
                var iterator = (SDBSongIterator)application.Database.QuerySongs($"ID = {songInfo.SongID}");
                if (iterator.EOF)
                    continue;

                var song = iterator.Item;
                song.PlayCounter += songInfo.PlayCount - songInfo.CommitedPlayCount;
                if (songInfo.LastPlayed.HasValue && songInfo.LastPlayed > song.LastPlayed)
                    song.LastPlayed = songInfo.LastPlayed.Value;
                songList.Add(song);
                iterator = null;
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);
            songList.UpdateAll();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<PlaylistMetaData>> ListPlaylists()
        {
            return Task.Run<IEnumerable<PlaylistMetaData>>(() =>
            {
                return GetAllChildPlaylists(application.PlaylistByID[0]).Select(ToPlaylistMetaData).ToList();
            });
        }

        public Task Open([AllowNull]JToken profileData)
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcesses();
                if (!Process.GetProcessesByName("MediaMonkey").Any())
                    throw new InvalidOperationException($"MediaMonkey is not running.");

                application = new SDBApplication();
                application.ShutdownAfterDisconnect = false;
            });
        }

        public Task<Option<Profile>> TryRegister()
        {
            return Task.Run(() =>
            {
                var app = new SDBApplication();
                app.ShutdownAfterDisconnect = false;
                app = null;
                GC.Collect();
                return new Profile
                {
                    InterfaceID = ID,
                    Title = "Local MediaMonkey Instance",
                    SubTitle = "(MediaMonkey COM Interface)"
                }.ToOption();
            });
        }

        #endregion Public Methods

        #region Private Methods

        private static PlaylistMetaData ToPlaylistMetaData(SDBPlaylist playlist)
                    => new PlaylistMetaData
                    {
                        ID = playlist.ID,
                        Title = playlist.Title,
                    };

        private static Song ToSong(SDBSongData song)
            => new Song
            {
                ID = song.ID,
                FileFormat = Path.GetExtension(song.Path).TrimStart('.'),
                Title = song.Title ?? string.Empty,
                Artist = song.ArtistName ?? string.Empty,
                Album = song.AlbumName ?? string.Empty,
            };

        private static (Song, string) ToSongData(SDBSongData song) => (ToSong(song), song.Path);

        private IEnumerable<SDBPlaylist> GetAllChildPlaylists(SDBPlaylist rootPlaylist) => GetPlaylists(rootPlaylist.ChildPlaylists).SelectMany(o => GetAllPlaylists(o));

        private IEnumerable<SDBPlaylist> GetAllPlaylists(SDBPlaylist rootPlaylist) => rootPlaylist.Yield().Concat(GetAllChildPlaylists(rootPlaylist));

        private IEnumerable<SDBPlaylist> GetPlaylists(SDBPlaylists playlists)
        {
            for (var i = 0; i < playlists.Count; i++)
            {
                yield return playlists.Item[i];
            }
        }

        #endregion Private Methods
    }
}