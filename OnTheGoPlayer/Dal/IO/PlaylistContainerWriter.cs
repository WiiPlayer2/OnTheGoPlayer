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

        public void Dispose()
        {
        }

        public async Task Flush()
        {
            await Task.Run(() => writer.Flush());
        }

        public Task Write(IPlaylistContainer playlistContainer)
        {
            return Task.Run(async () =>
            {
                writer.Write(Constants.CONTAINER_VERSION);
                writer.Write(playlistContainer.Playlist.Name);
                await WriteSongsTable(playlistContainer);
                await WriteSongData(playlistContainer);
            });
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

        private async Task WriteSongData(IPlaylistContainer playlistContainer)
        {
            foreach (var song in playlistContainer.Playlist.Songs)
            {
                var stream = await playlistContainer.GetSongStream(song);
                await stream.CopyToAsync(writer.BaseStream);
            }
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

        #endregion Public Methods
    }
}