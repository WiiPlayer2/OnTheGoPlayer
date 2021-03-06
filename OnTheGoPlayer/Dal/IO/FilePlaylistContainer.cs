﻿using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    internal class FilePlaylistContainer : IOPlaylistContainer
    {
        #region Private Fields

        private readonly string filePath;

        #endregion Private Fields

        #region Public Constructors

        public FilePlaylistContainer(string filePath, PlaylistMetaData metaData, IEnumerable<SongDataEntry> songDataEntries)
            : base(metaData, songDataEntries)
        {
            this.filePath = filePath;
        }

        #endregion Public Constructors

        #region Public Methods

        public override Task<Stream> GetSongStream(Song song, CancellationToken token)
        {
            var entry = songDataEntries[song.ID];
            var fstream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<Stream>(new SubStream(fstream, entry.DataOffset, entry.DataLength, false));
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}