using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Dal
{
    interface IPlaylistContainerExporter
    {
        bool IsOpen { get; }

        Task<bool> TryOpen(Window ownerWindow);

        Task Open(string data);

        Task Close();

        Task<IEnumerable<(int ID, string Name)>> ListPlaylists();

        Task<IPlaylistContainer> ExportPlaylist(int id);
    }
}
