using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal class MMDBPlaylistSong
    {
        #region Public Properties

        public long IDPlaylist { get; set; }

        public long IDPlaylistSong { get; set; }

        public long IDSong { get; set; }

        public long SongOrder { get; set; }

        #endregion Public Properties
    }
}