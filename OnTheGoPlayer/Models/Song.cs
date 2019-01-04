using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Models
{
    public class Song
    {
        #region Public Properties

        public string Album { get; }
        public string Artist { get; }
        public string FileFormat { get; }
        public int ID { get; }

        public string Title { get; }

        #endregion Public Properties
    }
}