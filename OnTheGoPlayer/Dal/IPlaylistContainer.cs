using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    public interface IPlaylistContainer
    {
        #region Public Properties

        bool IsDirty { get; }
        Playlist Playlist { get; }

        #endregion Public Properties

        #region Public Methods

        Task<Stream> GetSongStream(Song song);

        void Save();

        #endregion Public Methods
    }
}