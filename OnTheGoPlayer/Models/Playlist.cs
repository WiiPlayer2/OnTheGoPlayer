using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Models
{
    class Playlist
    {
        public IReadOnlyList<Song> Songs { get; }

        public string Name { get; }
    }
}
