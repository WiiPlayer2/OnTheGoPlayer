using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal
{
    internal class FilesPlaylistContainer : StreamPlaylistContainer
    {
        #region Public Constructors

        public FilesPlaylistContainer(PlaylistMetaData metaData, IEnumerable<(Song Song, string)> songs)
            : base(metaData, songs, path => Task.Run<Stream>(() => File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
        {
        }

        #endregion Public Constructors
    }
}