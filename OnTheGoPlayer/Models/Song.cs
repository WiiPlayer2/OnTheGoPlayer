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

        public string Album { get; set; }

        public string Artist { get; set; }

        public string FileFormat { get; set; }

        public int ID { get; set; }

        public string Title { get; set; }

        #endregion Public Properties
    }
}