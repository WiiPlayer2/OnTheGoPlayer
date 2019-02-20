using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal.IO
{
    internal class StreamPlaylistContainer : IOPlaylistContainer
    {
        #region Private Fields

        private Stream stream;

        #endregion Private Fields

        #region Public Constructors

        public StreamPlaylistContainer(Stream baseStream, PlaylistMetaData metaData, IEnumerable<SongDataEntry> songDataEntries)
            : base(metaData, songDataEntries)
        {
            stream = baseStream;
        }

        #endregion Public Constructors

        #region Public Methods

        public override Task<Stream> GetSongStream(Song song)
        {
            var entry = songDataEntries[song.ID];
            return Task.FromResult<Stream>(new SubStream(stream, entry.DataOffset, entry.DataLength, true));
        }

        public override void Save()
        {
        }

        #endregion Public Methods
    }
}