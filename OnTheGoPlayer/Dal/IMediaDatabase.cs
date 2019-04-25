using Newtonsoft.Json.Linq;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    public interface IMediaDatabase
    {
        #region Public Properties

        Guid ID { get; }

        bool IsOpen { get; }

        #endregion Public Properties

        #region Public Methods

        Task Close();

        Task<IPlaylistContainer> ExportPlaylist(int id, IProgress<(double?, string)> progress);

        Task ImportSongInfo(IEnumerable<SongInfo> songInfos);

        Task<IEnumerable<PlaylistMetaData>> ListPlaylists();

        Task Open(JToken profileData);

        Task<Option<Profile>> TryRegister();

        #endregion Public Methods
    }

    internal interface IMediaDatabase<TProfileData> : IMediaDatabase
    {
        #region Public Methods

        Task Open(TProfileData profileData);

        new Task<Option<Profile<TProfileData>>> TryRegister();

        #endregion Public Methods
    }
}