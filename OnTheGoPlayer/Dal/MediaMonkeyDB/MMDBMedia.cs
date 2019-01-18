using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal class MMDBMedia
    {
        #region Private Enums

        public enum MediaDriveType
        {
            LocalDrive = 3,
        }

        #endregion Private Enums

        #region Public Properties

        public int DriveLetter { get; set; }

        public MediaDriveType DriveType { get; set; }

        public int IDMedia { get; set; }

        #endregion Public Properties
    }
}