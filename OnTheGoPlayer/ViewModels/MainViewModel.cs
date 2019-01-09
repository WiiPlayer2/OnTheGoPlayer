using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheGoPlayer.ViewModels
{
    internal class MainViewModel
    {
        #region Public Properties

        public ImportViewModel ImportViewModel { get; } = new ImportViewModel();

        #endregion Public Properties
    }
}