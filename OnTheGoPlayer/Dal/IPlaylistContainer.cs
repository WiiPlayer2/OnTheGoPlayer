﻿using OnTheGoPlayer.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    public interface IPlaylistContainer
    {
        #region Public Properties

        Playlist Playlist { get; }

        #endregion Public Properties

        #region Public Methods

        Task<Stream> GetSongStream(Song song, CancellationToken token);

        #endregion Public Methods
    }
}