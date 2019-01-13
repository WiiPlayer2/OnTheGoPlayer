using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Helpers
{
    internal static class Dialogs
    {
        #region Public Methods

        public static (bool Result, string Path) ShowLoadContainer()
        {
            var openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "container",
                Filter = "Playlist Container (*.container)|*.container",
                Multiselect = false,
            };
            var result = openFileDialog.ShowDialog(Application.Current.MainWindow) ?? false;

            return (result, openFileDialog.FileName);
        }

        public static (bool Result, string Path) ShowSaveContainer()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "container",
                Filter = "Playlist Container (*.container)|*.container",
            };
            var result = saveFileDialog.ShowDialog(Application.Current.MainWindow) ?? false;

            return (result, saveFileDialog.FileName);
        }

        #endregion Public Methods
    }
}