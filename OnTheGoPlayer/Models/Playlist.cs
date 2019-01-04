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

        public string Name { get; }
        public IReadOnlyList<Song> Songs { get; }

        #endregion Public Properties
    }
}