using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Models
{
    public class SongInfo
    {
        #region Public Properties

        [NotNull]
        public int CommitedPlayCount { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public DateTime? LastPlayed { get; set; }

        [NotNull]
        public int PlayCount { get; set; }

        [NotNull]
        public int SongID { get; set; }

        #endregion Public Properties
    }
}