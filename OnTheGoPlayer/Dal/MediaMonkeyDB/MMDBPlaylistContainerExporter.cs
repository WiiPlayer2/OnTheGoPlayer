using Microsoft.Win32;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Collations;
using OnTheGoPlayer.Dal.MediaMonkeyDB.Queries;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Dal.MediaMonkeyDB
{
    internal class MMDBPlaylistContainerExporter : BaseDBMediaDatabase<string>
    {
        #region Private Fields

        private System.Data.SQLite.SQLiteConnection connection;

        #endregion Private Fields

        #region Public Constructors

        static MMDBPlaylistContainerExporter()
        {
            CollationsHelper.InitSQLiteFunctions();
        }

        #endregion Public Constructors

        #region Public Properties

        public override Guid ID => new Guid("49881597-f38a-4ed9-9aa2-d350b22b59fa");

        #endregion Public Properties

        #region Public Methods

        public override Task Open(string profileData) => OpenDatabase(profileData);

        public override Task<Option<Profile<string>>> TryRegister()
        {
            var openFileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "db",
                Filter = "MediaMonkey Database Files (*.db)|*.db",
                Multiselect = false,
            };
            var result = openFileDialog.ShowDialog() ?? false;
            if (!result)
                return Task.FromResult(Option<Profile<string>>.None);

            return Task.FromResult(new Profile<string>
            {
                InterfaceID = ID,
                Title = "Local MediaMonkey Database",
                SubTitle = openFileDialog.FileName,
                ProfileData = openFileDialog.FileName,
            }.ToOption());
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Task<string> FindMap(MMDBSong song)
        {
            var drives = DriveInfo.GetDrives();
            // TODO check media data first (DriveLetter, DriveLabel)
            foreach (var drive in drives)
            {
                var path = GetPath(song, drive.Name);
                if (File.Exists(path))
                {
                    return Task.FromResult(drive.Name);
                }
            }
            throw new NotImplementedException();
        }

        protected override string GetPath(MMDBSong song, string mappedMediaName)
        {
            var driveBaseName = mappedMediaName.Substring(0, mappedMediaName.LastIndexOf(':'));
            return $"{driveBaseName}{song.SongPath}";
        }

        protected override Task<Stream> GetStream(string path, CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        #endregion Protected Methods
    }
}