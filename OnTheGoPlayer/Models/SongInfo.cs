using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Models
{
    internal class SongInfo
    {
        #region Public Properties

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public DateTime? LastPlayed { get; set; }

        public int PlayCount { get; set; }

        public int SongID { get; set; }

        #endregion Public Properties
    }
}