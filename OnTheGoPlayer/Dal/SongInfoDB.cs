using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using SQLite;

namespace OnTheGoPlayer.Dal
{
    internal class SongInfoDB
    {
        #region Private Fields

        private SQLiteAsyncConnection connection;

        #endregion Private Fields

        #region Private Constructors

        private SongInfoDB()
        {
            var assembly = Assembly.GetEntryAssembly();
            var company = assembly.GetValue((AssemblyCompanyAttribute o) => o.Company);
            var product = assembly.GetValue((AssemblyProductAttribute o) => o.Product);
            var dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), company, product);
            var dbPath = Path.Combine(dataFolder, "main.db");
            Directory.CreateDirectory(dataFolder);

            connection = new SQLiteAsyncConnection(dbPath);
            CreateSchema();
        }

        #endregion Private Constructors

        #region Public Properties

        public static SongInfoDB Instance { get; } = new SongInfoDB();

        #endregion Public Properties

        #region Public Methods

        public async Task CommitInformation() => await connection.ExecuteAsync("UPDATE SongInfo SET CommitedPlayCount = SongInfo.PlayCount;");

        public async Task<SongInfo> Get(int songId)
        {
            var info = await connection.FindAsync<SongInfo>(o => o.SongID == songId);
            if (info == null)
                info = new SongInfo();
            return info;
        }

        public async Task<IReadOnlyList<SongInfo>> GetAll() => (await connection.QueryAsync<SongInfo>("SELECT * FROM SongInfo;")).ToList();

        public async Task<IReadOnlyList<SongInfo>> GetAllChangedInformation()
            => (await connection.QueryAsync<SongInfo>("SELECT * FROM SongInfo WHERE SongInfo.PlayCount > SongInfo.CommitedPlayCount;")).ToList();

        public async void IncreaseCounter(Song song)
        {
            var insert = false;
            var info = await connection.FindAsync<SongInfo>(o => o.SongID == song.ID);
            if (info == null)
            {
                insert = true;
                info = new SongInfo();
            }

            info.SongID = song.ID;
            info.PlayCount++;
            info.LastPlayed = DateTime.Now;

            if (insert)
                await connection.InsertAsync(info);
            else
                await connection.UpdateAsync(info);
        }

        #endregion Public Methods

        #region Private Methods

        private async void CreateSchema()
        {
            await connection.CreateTableAsync<SongInfo>();
        }

        #endregion Private Methods
    }
}