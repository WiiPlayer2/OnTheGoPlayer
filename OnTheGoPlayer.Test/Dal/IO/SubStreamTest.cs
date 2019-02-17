using OnTheGoPlayer.Dal.IO;
using OnTheGoPlayer.Test.Helpers.Extensions;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace OnTheGoPlayer.Test.Dal.IO
{
    [TestFixture]
    public class SubStreamTest
    {
        #region Public Methods

        [Test]
        public void WithFullRange_ShouldEqualFullStream()
        {
            var memStream = CreateMemoryStream(0, 100);
            var subStream = CreateSubStream(100, 0, 100);

            subStream.Should().Equal(memStream);
        }

        [Test]
        public void WithNoRange_ShouldEqualEmpty()
        {
            var memStream = CreateMemoryStream(0, 0);
            var subStream = CreateSubStream(100, 0, 0);

            subStream.Should().Equal(memStream);
        }

        [Test]
        public void WithRangeAtBeginning_ShouldEqualBeginning()
        {
            var memStream = CreateMemoryStream(0, 50);
            var subStream = CreateSubStream(100, 0, 50);

            subStream.Should().Equal(memStream);
        }

        [Test]
        public void WithRangeAtEnd_ShouldEqualEnd()
        {
            var memStream = CreateMemoryStream(50, 50);
            var subStream = CreateSubStream(100, 50, 50);

            subStream.Should().Equal(memStream);
        }

        [Test]
        public void WithRangeInMiddle_ShouldEqualMiddle()
        {
            var memStream = CreateMemoryStream(25, 50);
            var subStream = CreateSubStream(100, 25, 50);

            subStream.Should().Equal(memStream);
        }

        #endregion Public Methods

        #region Private Methods

        private MemoryStream CreateMemoryStream(int start, int count)
        {
            return new MemoryStream(Enumerable.Range(start, count).Select(o => (byte)o).ToArray());
        }

        private SubStream CreateSubStream(int count, int offset, int length)
        {
            return new SubStream(CreateMemoryStream(0, count), offset, length, false);
        }

        #endregion Private Methods
    }
}