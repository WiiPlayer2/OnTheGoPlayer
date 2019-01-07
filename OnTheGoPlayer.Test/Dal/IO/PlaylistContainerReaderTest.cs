using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Test.Helpers.Extensions;
using Resourcer;
using System.IO;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Dal.IO
{
    [TestClass]
    public class PlaylistContainerReaderTest
    {
        private static string filePath;

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            filePath = Path.GetTempFileName();
            using (var resStream = Resource.AsStream("test.container"))
            using (var fStream = File.Create(filePath))
            {
                resStream.CopyTo(fStream);
                fStream.Flush();
                fStream.Close();
                resStream.Close();
            }
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            File.Delete(filePath);
        }

        #region Public Methods

        [TestMethod]
        public async Task Read_WithTestDataAsStream()
        {
            var stream = Resource.AsStream("test.container");
            var reader = new PlaylistContainerReader(stream);

            var result = await reader.Read();

            result.Should().BeEquivalentTo(PlaylistContainerTestData.Data);
        }


        [TestMethod]
        public async Task Read_WithTestDataAsFile()
        {
            var reader = new PlaylistContainerReader(filePath);

            var result = await reader.Read();

            result.Should().BeEquivalentTo(PlaylistContainerTestData.Data);
        }

        #endregion Public Methods
    }
}