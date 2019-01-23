using OnTheGoPlayer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    internal class SongInfoDB
    {
        #region Private Fields

        private SQLiteAsyncConnection connection;

        #endregion Private Fields

        #region Public Constructors

        public SongInfoDB()
        {
            connection = new SQLiteAsyncConnection("./main.db");
            CreateSchema();
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<SongInfo> Get(int songId)
        {
            var info = await connection.FindAsync<SongInfo>(o => o.SongID == songId);
            if (info == null)
                info = new SongInfo();
            return info;
        }

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