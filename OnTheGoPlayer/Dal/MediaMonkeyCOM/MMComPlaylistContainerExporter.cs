using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OnTheGoPlayer.Helpers;
using SongsDB;

namespace OnTheGoPlayer.Dal.MediaMonkeyCOM
{
    internal class MMComPlaylistContainerExporter : IPlaylistContainerExporter
    {
        #region Private Fields

        private SDBApplication application;

        #endregion Private Fields

        #region Public Properties

        public bool IsOpen => application != null;

        #endregion Public Properties

        #region Public Methods

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task<IPlaylistContainer> ExportPlaylist(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(int ID, string Name)>> ListPlaylists()
        {
            return Task.Run<IEnumerable<(int ID, string Name)>>(() =>
            {
                return GetPlaylists(application.PlaylistByID[0].ChildPlaylists).Select(o => (ID: o.ID, Name: o.Title)).ToList();
            });
        }

        public Task Open(string data) => TryOpen(null);

        public Task<bool> TryOpen(Window ownerWindow)
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcesses();
                if (!Process.GetProcessesByName("MediaMonkey").Any())
                    return false;

                try
                {
                    application = new SDBApplication();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<SDBPlaylist> GetAllPlaylists(SDBPlaylist rootPlaylist)
        {
            return rootPlaylist.Yield().Concat(GetPlaylists(rootPlaylist.ChildPlaylists).SelectMany(o => GetAllPlaylists(o)));
        }

        private IEnumerable<SDBPlaylist> GetPlaylists(SDBPlaylists playlists)
        {
            for (var i = 0; i < playlists.Count; i++)
            {
                yield return playlists.Item[i];
            }
        }

        #endregion Private Methods
    }
}