using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Models
{
    public class Playlist
    {
        #region Public Properties

        public string Name { get; set; }
        public IReadOnlyList<Song> Songs { get; set; }

        #endregion Public Properties
    }
}