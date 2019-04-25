using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    internal class SongInfoWriter : IDisposable
    {
        #region Private Fields

        private BinaryWriter writer;

        #endregion Private Fields

        #region Public Constructors

        public SongInfoWriter(string filePath)
            : this(File.Create(filePath)) { }

        public SongInfoWriter(Stream outputStream)
        {
            writer = new BinaryWriter(outputStream);
        }

        #endregion Public Constructors

        #region Public Methods

        public static async Task Write(string filePath, IEnumerable<SongInfo> songInfos)
        {
            using (var writer = new SongInfoWriter(filePath))
                await writer.Write(songInfos);
        }

        public void Dispose()
        {
        }

        public async Task Write(IEnumerable<SongInfo> songInfos)
        {
            var list = songInfos.ToList();
            writer.Write(list.Count);
            foreach (var info in list)
            {
                await Write(info);
            }
            writer.Flush();
        }

        #endregion Public Methods

        #region Private Methods

        private Task Write(SongInfo info)
        {
            writer.Write(info.ID);
            writer.Write(info.SongID);
            writer.Write(info.PlayCount);
            writer.Write(info.CommitedPlayCount);
            writer.Write(info.LastPlayed?.Ticks ?? 0);
            return Task.CompletedTask;
        }

        #endregion Private Methods
    }
}