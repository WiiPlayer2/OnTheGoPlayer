using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheGoPlayer.ViewModels
{
    internal class MainViewModel
    {
        #region Public Properties

        public ExportViewModel ExportViewModel { get; } = new ExportViewModel();

        public PlayerViewModel PlayerViewModel { get; } = new PlayerViewModel();

        #endregion Public Properties
    }
}