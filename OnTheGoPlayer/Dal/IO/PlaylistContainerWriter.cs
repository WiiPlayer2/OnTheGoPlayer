using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    public class PlaylistContainerWriter : IDisposable
    {
        #region Private Fields

        private BinaryWriter writer;

        #endregion Private Fields

        #region Public Constructors

        public PlaylistContainerWriter(string filePath)
            : this(File.Create(filePath)) { }

        public PlaylistContainerWriter(Stream outputStream)
        {
            writer = new BinaryWriter(outputStream);
        }

        #endregion Public Constructors

        #region Public Methods

        public static async Task Write(string filePath, IPlaylistContainer playlistContainer, IProgress<(double?, string)> progress)
        {
            using (var writer = new PlaylistContainerWriter(filePath))
            {
                await writer.Write(playlistContainer, progress);
            }
        }

        public void Dispose()
        {
        }

        public async Task Flush()
        {
            await Task.Run(() => writer.Flush());
        }

        public Task Write(IPlaylistContainer playlistContainer, IProgress<(double?, string)> progress)
        {
            progress.Report((null, "Writing meta data..."));
            return Task.Run(async () =>
            {
                writer.Write(Constants.CONTAINER_VERSION);
                WriteMetaData(playlistContainer.Playlist.MetaData);
                await WriteSongsTable(playlistContainer);
                await WriteSongData(playlistContainer, progress);
            });
        }

        #endregion Public Methods

        #region Private Methods

        private void WriteMetaData(PlaylistMetaData metaData)
        {
            writer.Write(metaData.Title);
        }

        private void WriteSong(Song song, long offset, long length)
        {
            writer.Write(song.ID);
            writer.Write(offset);
            writer.Write(length);
            writer.Write(song.FileFormat);
            writer.Write(song.Title);
            writer.Write(song.Artist);
            writer.Write(song.Album);
        }

        private async Task WriteSongData(IPlaylistContainer playlistContainer, IProgress<(double?, string)> progress)
        {
            var i = 0;
            var count = playlistContainer.Playlist.Songs.Count;
            foreach (var song in playlistContainer.Playlist.Songs)
            {
                progress.Report((((double)i) / count, $"Writing song {song.Artist} - {song.Title} ({i + 1}/{count})..."));
                var stream = await playlistContainer.GetSongStream(song);
                await stream.CopyToAsync(writer.BaseStream);

                i++;
            }
            progress.Report((1, string.Empty));
        }

        private async Task WriteSongsTable(IPlaylistContainer playlistContainer)
        {
            writer.Write(playlistContainer.Playlist.Songs.Count);
            var currentOffset = 0L;
            foreach (var song in playlistContainer.Playlist.Songs)
            {
                var stream = await playlistContainer.GetSongStream(song);
                WriteSong(song, currentOffset, stream.Length);
                currentOffset += stream.Length;
            }
        }

        #endregion Private Methods
    }
}