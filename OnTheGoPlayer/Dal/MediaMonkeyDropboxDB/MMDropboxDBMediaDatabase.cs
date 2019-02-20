using System.Threading.Tasks;
using System.Windows;
using OnTheGoPlayer.Dal.MediaMonkeyDB;

namespace OnTheGoPlayer.Dal.MediaMonkeyDropboxDB
{
    using System.IO;
    using Dropbox.Api;
    using OnTheGoPlayer.Helpers;

    internal class MMDropboxDBMediaDatabase : BaseDBMediaDatabase
    {
        private DropboxClient client;

        #region Public Methods

        public override async Task<bool> TryOpen(Window ownerWindow)
        {
            var authDialog = new DropboxAuthDialog();
            if (!authDialog.ShowDialog() ?? false)
                return false;

            client = new DropboxClient(authDialog.Response.AccessToken);
            var selectDialog = new DropboxSelectDatabaseDialog(client);
            if (!selectDialog.ShowDialog() ?? false)
                return false;

            await Open(selectDialog.LocalDatabasePath);
            return IsOpen;
        }

        protected override async Task<Stream> GetStream(string path)
        {
            var downloadResponse = await client.Files.DownloadAsync(path);
            var downloadStream = await downloadResponse.GetContentAsStreamAsync();
            var bufferStream = new BufferStream(downloadStream, (int)downloadResponse.Response.Size);
            return bufferStream;
        }

        protected override async Task<string> FindMap(MMDBSong song)
        {
            if (!song.SongPath.ToLower().Contains("dropbox"))
                throw new FileNotFoundException($"Dropbox folder not found in song path of song #{song.ID}");

            var pathStartIndex = song.SongPath.ToLower().IndexOf("dropbox") + 7;
            var mappedName = song.SongPath.ToLower().Substring(0, pathStartIndex);
            var dropboxPath = GetPath(song, mappedName);
            var metadata = await client.Files.GetMetadataAsync(dropboxPath);

            return mappedName;
        }

        protected override string GetPath(MMDBSong song, string mappedMediaName)
        {
            if (!song.SongPath.ToLower().StartsWith(mappedMediaName))
                throw new FileNotFoundException($"Could not find song #{song.ID}");

            return song.SongPath.Remove(0, mappedMediaName.Length).Replace('\\', '/');
        }

        #endregion Public Methods
    }
}