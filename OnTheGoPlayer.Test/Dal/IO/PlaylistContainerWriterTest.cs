using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Test.Helpers.Extensions;
using Resourcer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Dal.IO
{
    [TestClass]
    public class PlaylistContainerWriterTest
    {
        #region Public Methods

        [TestMethod]
        public async Task Write_WithTestData()
        {
            var memStream = new MemoryStream();
            var writer = new PlaylistContainerWriter(memStream);

            await writer.Write(PlaylistContainerTestData.Data);
            await writer.Flush();

            memStream.Should().Equal(Resource.AsStream("test.container"));
        }

        //[TestMethod]
        public async Task WriteTestData()
        {
            Console.WriteLine(Environment.CurrentDirectory);
            using (var writer = new PlaylistContainerWriter("../../Dal/IO/test.container"))
            {
                await writer.Write(PlaylistContainerTestData.Data);
                await writer.Flush();
            }
        }

        #endregion Public Methods
    }
}