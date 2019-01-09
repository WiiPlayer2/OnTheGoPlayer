﻿using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Dal
{
    internal interface IPlaylistContainerExporter
    {
        #region Public Properties

        bool IsOpen { get; }

        #endregion Public Properties

        #region Public Methods

        Task Close();

        Task<IPlaylistContainer> ExportPlaylist(int id);

        Task<IEnumerable<PlaylistMetaData>> ListPlaylists();

        Task Open(string data);

        Task<bool> TryOpen(Window ownerWindow);

        #endregion Public Methods
    }
}