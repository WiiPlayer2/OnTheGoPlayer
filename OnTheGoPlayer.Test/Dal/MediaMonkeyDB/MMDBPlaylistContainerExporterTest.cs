using FluentAssertions;
using NUnit.Framework;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using OnTheGoPlayer.Models;
using Resourcer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Dal.MediaMonkeyDB
{
    [TestFixture, Ignore("postponed", Until = "2019-03-01")]
    public class MMDBPlaylistContainerExporterTest
    {
        #region Private Fields

        private static string dbFilepath;

        #endregion Private Fields

        #region Public Methods

        [OneTimeTearDown]
        public static void Cleanup()
        {
            File.Delete(dbFilepath);
        }

        [OneTimeSetUp]
        public static void Initialize()
        {
            dbFilepath = Path.GetTempFileName();
            using (var resStream = Resource.AsStream("mm-test.db"))
            using (var fStream = File.Create(dbFilepath))
            {
                resStream.CopyTo(fStream);
                fStream.Flush();
                fStream.Close();
                resStream.Close();
            }
        }

        [Test]
        public async Task IsOpen_AfterClose_ReturnsFalse()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            await exporter.Open(dbFilepath);
            await exporter.Close();

            exporter.IsOpen.Should().BeFalse();
        }

        [Test]
        public async Task IsOpen_AfterOpenWithTestDatabase_ReturnsTrue()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            await exporter.Open(dbFilepath);

            exporter.IsOpen.Should().BeTrue();
        }

        [Test]
        public void IsOpen_BeforeOpen_ReturnsFalse()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            exporter.IsOpen.Should().BeFalse();
        }

        [Test]
        public async Task ListPlaylists_WithTestDatabase_ReturnsCorrectEntries()
        {
            var exporter = new MMDBPlaylistContainerExporter();
            var shouldBeEntries = new[]
            {
                new PlaylistMetaData{ ID = 16, Title = "Camille" },
                new PlaylistMetaData{ ID = 19, Title = "DemFeels" },
                new PlaylistMetaData{ ID = 21, Title = "Do not forget" },
                new PlaylistMetaData{ ID = 23, Title = "Kled" },
            };

            await exporter.Open(dbFilepath);
            var entries = await exporter.ListPlaylists();

            entries.Should().BeEquivalentTo(shouldBeEntries);
        }

        [Test]
        public void Open_WithTestDatabase_DoesNotThrowException()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            Func<Task> act = async () => await exporter.Open(dbFilepath);

            act.Should().NotThrow();
        }

        #endregion Public Methods
    }
}