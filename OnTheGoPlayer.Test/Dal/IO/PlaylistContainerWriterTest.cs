using FluentAssertions;
using NSubstitute;
using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Test.Helpers.Extensions;
using Resourcer;
using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OnTheGoPlayer.Test.Dal.IO
{
    [TestFixture]
    public class PlaylistContainerWriterTest
    {
        #region Public Methods

        [Test]
        public async Task Write_WithTestData()
        {
            var memStream = new MemoryStream();
            var writer = new PlaylistContainerWriter(memStream);
            var progress = Substitute.For<IProgress<(double?, string)>>();

            await writer.Write(PlaylistContainerTestData.Data, progress);
            await writer.Flush();

            memStream.Should().Equal(Resource.AsStream("test.container"));
        }

        //[Test]
        public async Task WriteTestData()
        {
            Console.WriteLine(Environment.CurrentDirectory);
            var progress = Substitute.For<IProgress<(double?, string)>>();
            using (var writer = new PlaylistContainerWriter("../../Dal/IO/test.container"))
            {
                await writer.Write(PlaylistContainerTestData.Data, progress);
                await writer.Flush();
            }
        }

        #endregion Public Methods
    }
}