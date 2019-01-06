using OnTheGoPlayer.Dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Helpers.Extensions
{
    internal static class AssertionExtensions
    {
        #region Public Methods

        public static PlaylistContainerAssertions Should(this IPlaylistContainer actualValue) => new PlaylistContainerAssertions(actualValue);

        public static StreamAssertions Should(this Stream actualValue) => new StreamAssertions(actualValue);

        #endregion Public Methods
    }
}