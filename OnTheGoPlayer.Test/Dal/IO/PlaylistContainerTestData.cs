﻿using NSubstitute;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Models;
using System.IO;
using System.Linq;
using System.Threading;

namespace OnTheGoPlayer.Test.Dal.IO
{
    internal static class PlaylistContainerTestData
    {
        #region Public Properties

        public static IPlaylistContainer Data
        {
            get
            {
                var ret = Substitute.For<IPlaylistContainer>();
                ret.Playlist.Returns(new Playlist
                {
                    MetaData = new PlaylistMetaData
                    {
                        Title = "Test Playlist",
                    },
                    Songs = Enumerable.Range(0, 2).Select(id => new Song
                    {
                        ID = id,
                        Title = $"Song #{id}",
                        Album = "Test Album",
                        Artist = "Test Artist",
                        FileFormat = "bin",
                    }).ToList(),
                });
                ret.GetSongStream(null, CancellationToken.None).ReturnsForAnyArgs(callInfo =>
                    new MemoryStream(Enumerable.Range(0, 256).Select(o => (byte)o).ToArray()));
                return ret;
            }
        }

        #endregion Public Properties
    }
}