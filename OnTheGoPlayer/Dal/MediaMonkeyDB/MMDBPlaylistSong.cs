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

        public int IDPlaylist { get; set; }

        public int IDPlaylistSong { get; set; }

        public int IDSong { get; set; }

        public int SongOrder { get; set; }

        #endregion Public Properties
    }
}