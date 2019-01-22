using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal class MMDBSong
    {
        #region Public Properties

        public string Album { get; set; }

        public string Artist { get; set; }

        public long ID { get; set; }

        public long IDMedia { get; set; }

        public string SongPath { get; set; }

        public string SongTitle { get; set; }

        #endregion Public Properties
    }
}