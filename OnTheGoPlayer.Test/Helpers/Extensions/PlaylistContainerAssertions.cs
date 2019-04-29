using FluentAssertions;
using FluentAssertions.Primitives;
using OnTheGoPlayer.Dal;
using System.Linq;
using System.Threading;

namespace OnTheGoPlayer.Test.Helpers.Extensions
{
    internal class PlaylistContainerAssertions : ReferenceTypeAssertions<IPlaylistContainer, PlaylistContainerAssertions>
    {
        #region Private Fields

        private readonly IPlaylistContainer actualContainer;

        #endregion Private Fields

        #region Public Constructors

        public PlaylistContainerAssertions(IPlaylistContainer actualValue)
        {
            actualContainer = actualValue;
        }

        #endregion Public Constructors

        #region Protected Properties

        protected override string Identifier => $"{actualContainer?.GetType()}";

        #endregion Protected Properties

        #region Public Methods

        [CustomAssertion]
        public void BeEquivalentTo(IPlaylistContainer expectedValue)
        {
            new ObjectAssertions(expectedValue).BeEquivalentTo(expectedValue);

            var actualSongs = actualContainer.Playlist.Songs.OrderBy(o => o.ID);
            var expectedSongs = expectedValue.Playlist.Songs.OrderBy(o => o.ID);
            var zipped = actualSongs.Zip(expectedSongs, (left, right) => (actual: left, expected: right));

            foreach (var pair in zipped)
            {
                actualContainer.GetSongStream(pair.actual, CancellationToken.None).Result.Should().Equal(expectedValue.GetSongStream(pair.expected, CancellationToken.None).Result);
            }
        }

        #endregion Public Methods
    }
}