using FluentAssertions;
using FluentAssertions.Primitives;
using System.IO;

namespace OnTheGoPlayer.Test.Helpers.Extensions
{
    public class StreamAssertions : ReferenceTypeAssertions<StreamAssertions, StreamAssertions>
    {
        #region Private Fields

        private readonly Stream stream;

        #endregion Private Fields

        #region Public Constructors

        public StreamAssertions(Stream stream)
        {
            this.stream = stream;
        }

        #endregion Public Constructors

        #region Protected Properties

        protected override string Identifier => $"{stream?.GetType()}";

        #endregion Protected Properties

        #region Public Methods

        [CustomAssertion]
        public void Equal(Stream expectedStream)
        {
            if (ReferenceEquals(stream, expectedStream))
                return;

            stream.CanRead.Should().BeTrue();
            expectedStream.CanRead.Should().BeTrue();

            var actualData = GetData(stream);
            var expectedData = GetData(expectedStream);

            actualData.Should().Equal(expectedData);
        }

        #endregion Public Methods

        #region Private Methods

        private static byte[] GetData(Stream stream)
        {
            if (stream is MemoryStream memStream)
                return memStream.ToArray();

            if (stream.CanSeek)
                stream.Position = 0;
            using (memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                memStream.Flush();
                return memStream.ToArray();
            }
        }

        #endregion Private Methods
    }
}