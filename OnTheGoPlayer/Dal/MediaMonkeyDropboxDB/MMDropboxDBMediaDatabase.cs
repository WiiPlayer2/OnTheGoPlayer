using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OnTheGoPlayer.Dal.MediaMonkeyDB;

namespace OnTheGoPlayer.Dal.MediaMonkeyDropboxDB
{
    using System;
    using System.IO;
    using Dropbox.Api;
    using OnTheGoPlayer.Helpers;

    internal class MMDropboxDBMediaDatabase : BaseDBMediaDatabase<DropboxProfileData>
    {
        #region Private Fields

        private DropboxClient client;

        #endregion Private Fields

        #region Public Properties

        public override Guid ID { get; } = new Guid("922c1a45-92fb-455c-90ba-55504bf9be17");

        #endregion Public Properties

        #region Public Methods

        public override async Task Open(DropboxProfileData profileData)
        {
            client = new DropboxClient(profileData.AccessToken);

            var downloadResponse = await client.Files.DownloadAsync(profileData.DatabasePath);
            var stream = await downloadResponse.GetContentAsStreamAsync();
            var tmpPath = Path.GetTempFileName();

            using (var fileStream = File.OpenWrite(tmpPath))
            {
                await stream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            await OpenDatabase(tmpPath);
        }

        public override async Task<Option<Profile<DropboxProfileData>>> TryRegister()
        {
            var authDialog = new DropboxAuthDialog();
            if (!authDialog.ShowDialog() ?? false)
                return Option<Profile<DropboxProfileData>>.None;

            var client = new DropboxClient(authDialog.Response.AccessToken);
            var selectDialog = new DropboxSelectDatabaseDialog(client);
            if (!selectDialog.ShowDialog() ?? false)
                return Option<Profile<DropboxProfileData>>.None;

            var account = await client.Users.GetCurrentAccountAsync();
            return new Profile<DropboxProfileData>
            {
                InterfaceID = ID,
                Title = $"Dropbox: {account.Name.DisplayName}",
                SubTitle = selectDialog.DatabasePath,
                ProfileData = new DropboxProfileData
                {
                    AccessToken = authDialog.Response.AccessToken,
                    DatabasePath = selectDialog.DatabasePath,
                },
            };
        }

        #endregion Public Methods

        #region Protected Methods

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

        protected override async Task<Stream> GetStream(string path, CancellationToken cancellationToken)
        {
            var downloadResponse = await TaskHelper.Retry(token => client.Files.DownloadAsync(path), TimeSpan.FromSeconds(1), cancellationToken);
            //var downloadStream = await downloadResponse.GetContentAsStreamAsync();
            //var bufferStream = new BufferStream(downloadStream, (int)downloadResponse.Response.Size);
            //return bufferStream;
            cancellationToken.ThrowIfCancellationRequested();

            var downloadData = await downloadResponse.GetContentAsByteArrayAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return new MemoryStream(downloadData)
            {
                Position = 0
            };
        }

        #endregion Protected Methods
    }
}