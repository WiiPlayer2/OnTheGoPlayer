using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal
{
    internal class StreamPlaylistContainer : IPlaylistContainer
    {
        #region Private Fields

        private readonly Func<string, Task<Stream>> getStreamFunc;

        private readonly Dictionary<int, (Song, string FilePath)> songData;

        #endregion Private Fields

        #region Public Constructors

        public StreamPlaylistContainer(PlaylistMetaData metaData, IEnumerable<(Song Song, string)> songs, Func<string, Task<Stream>> getStreamFunc)
        {
            songData = songs.ToDictionary(o => o.Song.ID);
            Playlist = new Playlist
            {
                MetaData = metaData,
                Songs = songs.Select(o => o.Song).ToList(),
            };
            this.getStreamFunc = getStreamFunc;
        }

        #endregion Public Constructors

        #region Public Properties

        public Playlist Playlist { get; }

        #endregion Public Properties

        #region Public Methods

        public async Task<Stream> GetSongStream(Song song)
        {
            var data = songData[song.ID];
            return await getStreamFunc(data.FilePath);
        }

        #endregion Public Methods
    }
}