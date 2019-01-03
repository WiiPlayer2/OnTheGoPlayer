using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    class PlaylistContainerReader : IDisposable
    {
        public PlaylistContainerReader(string filePath)
        {
            throw new NotImplementedException();
        }

        public PlaylistContainerReader(Stream inputStream)
        {
            throw new NotImplementedException();
        }

        public Task<IPlaylistContainer> Read()
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}
