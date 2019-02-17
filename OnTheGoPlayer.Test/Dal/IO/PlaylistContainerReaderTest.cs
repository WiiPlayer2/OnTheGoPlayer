using FluentAssertions;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Test.Helpers.Extensions;
using Resourcer;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OnTheGoPlayer.Test.Dal.IO
{
    [TestFixture]
    public class PlaylistContainerReaderTest
    {
        #region Private Fields

        private static string filePath;

        #endregion Private Fields

        //[OneTimeTearDown]
        //public static void Cleanup()
        //{
        //    File.Delete(filePath);
        //}

        #region Public Methods

        [OneTimeSetUp]
        public static void Initialize()
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

        [Test]
        public async Task Read_WithTestDataAsFile()
        {
            var reader = new PlaylistContainerReader(filePath);

            var result = await reader.Read();

            result.Should().BeEquivalentTo(PlaylistContainerTestData.Data);
        }

        [Test]
        public async Task Read_WithTestDataAsStream()
        {
            var stream = Resource.AsStream("test.container");
            var reader = new PlaylistContainerReader(stream);

            var result = await reader.Read();

            result.Should().BeEquivalentTo(PlaylistContainerTestData.Data);
        }

        #endregion Public Methods
    }
}