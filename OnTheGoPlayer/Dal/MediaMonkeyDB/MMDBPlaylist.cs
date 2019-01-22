using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal class MMDBPlaylist
    {
        #region Public Properties

        public long IDPlaylist { get; set; }

        public bool IsAutoPlaylist { get; set; }

        public string PlaylistName { get; set; }

        public string QueryData { get; set; }

        #endregion Public Properties
    }
}