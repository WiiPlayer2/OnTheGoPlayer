using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal.IO
{
    internal class FilePlaylistContainer : IOPlaylistContainer
    {
        #region Private Fields

        private readonly string filePath;

        #endregion Private Fields

        #region Public Constructors

        public FilePlaylistContainer(string filePath, long offset, string name, IEnumerable<IOPlaylistContainer.SongDataEntry> songDataEntries)
            : base(offset, name, songDataEntries)
        {
            this.filePath = filePath;
        }

        #endregion Public Constructors

        #region Public Methods

        public override Task<Stream> GetSongStream(Song song)
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}