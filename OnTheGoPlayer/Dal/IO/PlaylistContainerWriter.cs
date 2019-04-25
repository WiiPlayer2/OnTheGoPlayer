using OnTheGoPlayer.Models;
using System;
using System.IO;
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
                await writer.Flush();
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
                WriteSongsTable(playlistContainer);
                await WriteSongData(playlistContainer, progress);
            });
        }

        #endregion Public Methods

        #region Private Methods

        private void WriteMetaData(PlaylistMetaData metaData)
        {
            writer.Write(metaData.Title);
        }

        private void WriteSong(Song song)
        {
            writer.Write(song.ID);
            writer.Write(song.FileFormat);
            writer.Write(song.Title);
            writer.Write(song.Artist);
            writer.Write(song.Album);
        }

        private async Task WriteSongData(IPlaylistContainer playlistContainer, IProgress<(double?, string)> progress)
        {
            var i = 0;
            var count = playlistContainer.Playlist.Songs.Count;
            writer.Write(count);
            foreach (var song in playlistContainer.Playlist.Songs)
            {
                progress.Report((((double)i) / count, $"Writing song {song.Artist} - {song.Title} ({i + 1}/{count})..."));
                var stream = await playlistContainer.GetSongStream(song);
                writer.Write(song.ID);
                writer.Write(stream.Length);
                await stream.CopyToAsync(writer.BaseStream);

                i++;
            }
            progress.Report((1, string.Empty));
        }

        private void WriteSongsTable(IPlaylistContainer playlistContainer)
        {
            writer.Write(playlistContainer.Playlist.Songs.Count);
            foreach (var song in playlistContainer.Playlist.Songs)
            {
                WriteSong(song);
            }
        }

        #endregion Private Methods
    }
}