using System;
using SQLite;

namespace OnTheGoPlayer.Models
{
    public class SongInfo
    {
        #region Public Properties

        public int CommitedPlayCount { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public DateTime? LastPlayed { get; set; }

        public int PlayCount { get; set; }

        public int SongID { get; set; }

        #endregion Public Properties
    }
}