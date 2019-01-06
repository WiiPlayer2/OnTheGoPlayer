using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Test.Helpers.Extensions;
using Resourcer;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Dal.IO
{
    [TestClass]
    public class PlaylistContainerReaderTest
    {
        #region Public Methods

        [TestMethod]
        public async Task Read_WithTestData()
        {
            var stream = Resource.AsStream("test.container");
            var reader = new PlaylistContainerReader(stream);

            var result = await reader.Read();

            result.Should().BeEquivalentTo(PlaylistContainerTestData.Data);
        }

        #endregion Public Methods
    }
}