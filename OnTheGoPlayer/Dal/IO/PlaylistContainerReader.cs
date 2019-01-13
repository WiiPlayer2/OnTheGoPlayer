using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    public class PlaylistContainerReader : IDisposable
    {
        #region Private Fields

        private readonly string filePath;

        private BinaryReader reader;

        #endregion Private Fields

        #region Public Constructors

        public PlaylistContainerReader(string filePath)
            : this(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), false)
        {
            this.filePath = filePath;
        }

        public PlaylistContainerReader(Stream inputStream)
            : this(inputStream, true) { }

        #endregion Public Constructors

        #region Private Constructors

        private PlaylistContainerReader(Stream inputStream, bool leaveOpen)
        {
            reader = new BinaryReader(inputStream, Encoding.UTF8, leaveOpen);
        }

        #endregion Private Constructors

        #region Public Methods

        public static async Task<IPlaylistContainer> Read(string filePath)
        {
            using (var reader = new PlaylistContainerReader(filePath))
            {
                return await reader.Read();
            }
        }

        public void Dispose()
        {
        }

        public Task<IPlaylistContainer> Read()
        {
            return Task.Run<IPlaylistContainer>(() =>
            {
                var containerVersion = reader.ReadUInt16();
                if (containerVersion != Constants.CONTAINER_VERSION)
                    throw new InvalidDataException($"Invalid container version ({containerVersion})");

                var metaData = ReadPlaylistMetaData();
                var songEntries = ReadSongsTable();
                var offset = reader.BaseStream.Position;

                if (filePath == null)
                    return new StreamPlaylistContainer(reader.BaseStream, offset, metaData, songEntries);
                return new FilePlaylistContainer(filePath, offset, metaData, songEntries);
            });
        }

        #endregion Public Methods

        #region Private Methods

        private PlaylistMetaData ReadPlaylistMetaData()
        {
            var metaData = new PlaylistMetaData();
            metaData.Title = reader.ReadString();
            return metaData;
        }

        private FilePlaylistContainer.SongDataEntry ReadSongDataEntry()
        {
            var song = new FilePlaylistContainer.SongDataEntry();
            song.Song.ID = reader.ReadInt32();
            song.DataOffset = reader.ReadInt64();
            song.DataLength = reader.ReadInt64();
            song.Song.FileFormat = reader.ReadString();
            song.Song.Title = reader.ReadString();
            song.Song.Artist = reader.ReadString();
            song.Song.Album = reader.ReadString();
            return song;
        }

        private IEnumerable<FilePlaylistContainer.SongDataEntry> ReadSongsTable()
        {
            var count = reader.ReadInt32();
            return Enumerable.Range(0, count).Select(_ => ReadSongDataEntry()).ToList();
        }

        #endregion Private Methods
    }
}