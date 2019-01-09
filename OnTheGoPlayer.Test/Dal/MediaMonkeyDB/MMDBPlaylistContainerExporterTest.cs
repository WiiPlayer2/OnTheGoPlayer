﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using Resourcer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Dal.MediaMonkeyDB
{
    [TestClass]
    public class MMDBPlaylistContainerExporterTest
    {
        #region Private Fields

        private static string dbFilepath;

        #endregion Private Fields

        #region Public Methods

        [ClassCleanup]
        public static void Cleanup()
        {
            File.Delete(dbFilepath);
        }

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
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

        [TestMethod]
        public async Task IsOpen_AfterClose_ReturnsFalse()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            await exporter.Open(dbFilepath);
            await exporter.Close();

            exporter.IsOpen.Should().BeFalse();
        }

        [TestMethod]
        public async Task IsOpen_AfterOpenWithTestDatabase_ReturnsTrue()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            await exporter.Open(dbFilepath);

            exporter.IsOpen.Should().BeTrue();
        }

        [TestMethod]
        public void IsOpen_BeforeOpen_ReturnsFalse()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            exporter.IsOpen.Should().BeFalse();
        }

        [TestMethod]
        public async Task ListPlaylists_WithTestDatabase_ReturnsCorrectEntries()
        {
            var exporter = new MMDBPlaylistContainerExporter();
            var shouldBeEntries = new (int ID, string Name)[]
            {
                (16, "Camille"),
                (19, "DemFeels"),
                (21, "Do not forget"),
                (23, "Kled"),
            };

            await exporter.Open(dbFilepath);
            var entries = await exporter.ListPlaylists();

            entries.Should().BeEquivalentTo(shouldBeEntries);
        }

        [TestMethod]
        public void Open_WithTestDatabase_DoesNotThrowException()
        {
            var exporter = new MMDBPlaylistContainerExporter();

            Func<Task> act = async () => await exporter.Open(dbFilepath);

            act.Should().NotThrow();
        }

        #endregion Public Methods
    }
}