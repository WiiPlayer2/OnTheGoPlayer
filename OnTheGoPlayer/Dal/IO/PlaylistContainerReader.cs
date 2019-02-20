using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

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
            song.Song.FileFormat = reader.ReadString();
            song.Song.Title = reader.ReadString();
            song.Song.Artist = reader.ReadString();
            song.Song.Album = reader.ReadString();
            return song;
        }

        private IEnumerable<FilePlaylistContainer.SongDataEntry> ReadSongsTable()
        {
            var count = reader.ReadInt32();
            var entries = Enumerable.Range(0, count).Select(_ => ReadSongDataEntry()).ToList();
            var groups = entries.GroupBy(o => o.ID).ToDictionary(o => o.Key);

            count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var id = reader.ReadInt32();
                var length = reader.ReadInt64();

                foreach (var entry in groups[id])
                {
                    entry.DataOffset = reader.BaseStream.Position;
                    entry.DataLength = length;
                }

                reader.BaseStream.Seek(length, SeekOrigin.Current);
            }
            return entries;
        }

        #endregion Private Methods
    }
}