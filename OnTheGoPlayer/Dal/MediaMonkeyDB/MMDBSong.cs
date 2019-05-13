using OnTheGoPlayer.Helpers;
using System;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal class MMDBSong
    {
        #region Public Properties

        public string Album { get; set; }

        public string Artist { get; set; }

        public long ID { get; set; }

        public long IDMedia { get; set; }

        public DateTime? LastTimePlayedDateTime
        {
            get => LastTimePlayed.ToOption().Map(DateTime.FromOADate).GetValueOrDefault();
            set => LastTimePlayed = value.ToOption().Match<double?>(o => o.ToOADate(), () => null);
        }

        public int PlayCounter { get; set; }

        public string SongPath { get; set; }

        public string SongTitle { get; set; }

        #endregion Public Properties

        #region Private Properties

        private double? LastTimePlayed { get; set; }

        #endregion Private Properties
    }
}