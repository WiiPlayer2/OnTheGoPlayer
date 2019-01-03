using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    class PlaylistContainerWriter : IDisposable
    {
        public PlaylistContainerWriter(string filePath)
        {
            throw new NotImplementedException();
        }

        public PlaylistContainerWriter(Stream outputStream)
        {
            throw new NotImplementedException();
        }

        public void Write(IPlaylistContainer playlistContainer)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}
