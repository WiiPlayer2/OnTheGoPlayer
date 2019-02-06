using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    internal class SongInfoReader : IDisposable
    {
        #region Private Fields

        private BinaryReader reader;

        #endregion Private Fields

        #region Public Constructors

        public SongInfoReader(string filePath)
            : this(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) { }

        public SongInfoReader(Stream outputStream)
        {
            reader = new BinaryReader(outputStream);
        }

        #endregion Public Constructors

        #region Public Methods

        public static async Task<IEnumerable<SongInfo>> Read(string filePath)
        {
            using (var reader = new SongInfoReader(filePath))
                return await reader.Read();
        }

        public void Dispose()
        {
        }

        public async Task<IEnumerable<SongInfo>> Read()
        {
            var list = new List<SongInfo>();
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                list.Add(await ReadSongInfo());
            }
            return list;
        }

        public Task<SongInfo> ReadSongInfo()
        {
            var songInfo = new SongInfo();
            songInfo.ID = reader.ReadInt32();
            songInfo.SongID = reader.ReadInt32();
            songInfo.PlayCount = reader.ReadInt32();
            songInfo.CommitedPlayCount = reader.ReadInt32();
            var lastPlayedTicks = reader.ReadInt64();
            songInfo.LastPlayed = lastPlayedTicks == 0 ? (DateTime?)null : new DateTime(lastPlayedTicks);

            return Task.FromResult(songInfo);
        }

        #endregion Public Methods
    }
}