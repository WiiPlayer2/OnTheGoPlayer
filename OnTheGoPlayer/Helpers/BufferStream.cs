using System;

namespace OnTheGoPlayer.Helpers
{
    using System.IO;

    internal class BufferStream : Stream
    {
        private readonly Stream baseStream;

        private readonly byte[] buffer;

        private int currentMaxPosition = 0;

        public BufferStream(Stream baseStream, int length)
        {
            if (!baseStream.CanRead)
                throw new ArgumentException("Stream is not readable.", nameof(baseStream));

            this.baseStream = baseStream;
            buffer = new byte[length];
            Length = length;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        private void EnsureData(long maxPosition)
        {
            if (maxPosition <= currentMaxPosition)
                return;

            var needCount = maxPosition - currentMaxPosition;
            var readCount = baseStream.Read(buffer, currentMaxPosition, (int)needCount);
            currentMaxPosition += readCount;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var realCount = Math.Min(count, Length - Position);
            EnsureData(Position + realCount);

            Array.Copy(this.buffer, Position, buffer, offset, realCount);

            Position += realCount;
            return (int)realCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length { get; }

        public override long Position { get; set; }
    }
}