using System;

namespace OnTheGoPlayer.Helpers
{
    using System.IO;

    internal class BufferStream : Stream
    {
        private readonly Stream baseStream;

        private byte[] buffer;

        private int currentSize;

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

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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