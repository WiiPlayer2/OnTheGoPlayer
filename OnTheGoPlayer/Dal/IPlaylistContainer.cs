using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    interface IPlaylistContainer
    {
        Playlist Playlist { get; }

        bool IsDirty { get; }

        Task<Stream> GetSongStream(Song song);

        void Save();
    }
}
