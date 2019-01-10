using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using SongsDB;

namespace OnTheGoPlayer.Dal.MediaMonkeyCOM
{
    internal class MMComPlaylistContainerExporter : IPlaylistContainerExporter
    {
        #region Private Fields

        private SDBApplication application;

        #endregion Private Fields

        #region Public Properties

        public bool IsOpen => application != null;

        #endregion Public Properties

        #region Public Methods

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress)
        {
            progress.Report((null, $"Exporting playlist #{id}..."));
            return Task.Run<IPlaylistContainer>(() =>
            {
                var sdbPlaylist = application.PlaylistByID[id];
                var metaData = ToPlaylistMetaData(sdbPlaylist);

                var songs = Enumerable.Range(0, sdbPlaylist.Tracks.Count).Select(i =>
                {
                    progress.Report((((double)i) / sdbPlaylist.Tracks.Count, $"Reading song #{i + 1}..."));
                    var data = ToSongData(sdbPlaylist.Tracks.Item[i]);
                    return data;
                }).ToList();
                progress.Report((1, "Finalizing container..."));

                return new MMComPlaylistContainer(metaData, songs);
            });
        }

        public Task<IEnumerable<PlaylistMetaData>> ListPlaylists()
        {
            return Task.Run<IEnumerable<PlaylistMetaData>>(() =>
            {
                return GetAllChildPlaylists(application.PlaylistByID[0]).Select(ToPlaylistMetaData).ToList();
            });
        }

        public Task Open(string data) => TryOpen(null);

        public Task<bool> TryOpen(Window ownerWindow)
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcesses();
                if (!Process.GetProcessesByName("MediaMonkey").Any())
                    return false;

                try
                {
                    application = new SDBApplication();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
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

        private static MMComSongData ToSongData(SDBSongData song)
            => new MMComSongData
            {
                Song = ToSong(song),
                FilePath = song.Path,
            };

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