using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    interface IPlaylistContainerExporter
    {
        Task<(int ID, string Name)> ListPlaylists();

        Task<IPlaylistContainer> ExportPlaylist(int id);
    }
}
