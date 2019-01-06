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

        protected readonly long offset;

        #endregion Protected Fields

        #region Protected Constructors

        protected IOPlaylistContainer(long offset, string name, IEnumerable<SongDataEntry> songDataEntries)
        {
            this.offset = offset;
            Playlist = new Playlist
            {
                Name = name,
                Songs = songDataEntries.Select(o => o.Song).ToList(),
            };
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