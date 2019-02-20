using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheGoPlayer.Models;

namespace OnTheGoPlayer.Dal.IO
{
    internal abstract class IOPlaylistContainer : IPlaylistContainer
    {
        #region Protected Fields

        protected readonly Dictionary<int, SongDataEntry> songDataEntries;

        #endregion Protected Fields

        #region Protected Constructors

        protected IOPlaylistContainer(PlaylistMetaData metaData, IEnumerable<SongDataEntry> songDataEntries)
        {
            Playlist = new Playlist
            {
                MetaData = metaData,
                Songs = songDataEntries.Select(o => o.Song).ToList(),
            };
            this.songDataEntries = songDataEntries.ToDictionary(o => o.ID);
        }

        #endregion Protected Constructors

        #region Public Properties

        public bool IsDirty { get; private set; }

        public Playlist Playlist { get; }

        #endregion Public Properties

        #region Public Methods

        public abstract Task<Stream> GetSongStream(Song song);

        public abstract void Save();

        #endregion Public Methods

        #region Public Classes

        public class SongDataEntry
        {
            #region Public Properties

            public long DataLength { get; set; }

            public long DataOffset { get; set; }

            public int ID => Song.ID;

            public Song Song { get; set; } = new Song();

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}