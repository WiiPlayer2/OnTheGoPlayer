using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal.MediaMonkeyCOM
{
    internal class MMComPlaylistContainer : IPlaylistContainer
    {
        #region Private Fields

        private readonly Dictionary<int, MMComSongData> songData;

        #endregion Private Fields

        #region Public Constructors

        public MMComPlaylistContainer(PlaylistMetaData metaData, IEnumerable<MMComSongData> songs)
        {
            songData = songs.ToDictionary(o => o.Song.ID);
            Playlist = new Playlist
            {
                MetaData = metaData,
                Songs = songs.Select(o => o.Song).ToList(),
            };
        }

        #endregion Public Constructors

        #region Public Properties

        public Playlist Playlist { get; }

        #endregion Public Properties

        #region Public Methods

        public Task<Stream> GetSongStream(Song song)
        {
            return Task.Run<Stream>(() =>
            {
                var data = songData[song.ID];
                return File.Open(data.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            });
        }

        #endregion Public Methods
    }
}