using OnTheGoPlayer.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    internal class FilesPlaylistContainer : StreamPlaylistContainer
    {
        #region Public Constructors

        public FilesPlaylistContainer(PlaylistMetaData metaData, IEnumerable<(Song Song, string)> songs)
            : base(metaData, songs, (path, _) => Task.Run<Stream>(() => File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
        {
        }

        #endregion Public Constructors
    }
}