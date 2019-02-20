using System;

namespace OnTheGoPlayer.Helpers
{
    using System.IO;

    internal class BufferStream : Stream
    {
        #region Private Fields

        private readonly Stream baseStream;

        private readonly byte[] buffer;

        private int currentMaxPosition = 0;

        #endregion Private Fields

        #region Public Constructors

        public BufferStream(Stream baseStream, int length)
        {
            if (!baseStream.CanRead)
                throw new ArgumentException("Stream is not readable.", nameof(baseStream));

            this.baseStream = baseStream;
            buffer = new byte[length];
            Length = length;
        }

        #endregion Public Constructors

        #region Public Properties

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length { get; }

        public override long Position { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var realCount = Math.Min(count, Length - Position);
            EnsureData(Position + realCount);

            Array.Copy(this.buffer, Position, buffer, offset, realCount);

            Position += realCount;
            return (int)realCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var newPos = Position + offset;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPos = offset;
                    break;

                case SeekOrigin.End:
                    newPos = Length + offset;
                    break;
            }

            if (newPos > Length)
                throw new NotSupportedException("Cannot seek behind the stream.");

            EnsureData(newPos);
            Position = newPos;
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion Public Methods

        #region Private Methods

        private void EnsureData(long maxPosition)
        {
            if (maxPosition <= currentMaxPosition)
                return;

            var needCount = maxPosition - currentMaxPosition;
            var readCount = baseStream.Read(buffer, currentMaxPosition, (int)needCount);
            currentMaxPosition += readCount;
        }

        #endregion Private Methods
    }
}