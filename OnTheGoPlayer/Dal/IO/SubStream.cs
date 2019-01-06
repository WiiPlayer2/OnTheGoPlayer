using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal.IO
{
    [Janitor.SkipWeaving]
    internal class SubStream : Stream
    {
        #region Private Fields

        private readonly bool leaveOpen;
        private readonly long offset;
        private Stream stream;

        #endregion Private Fields

        #region Public Properties

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length { get; }

        public override long Position
        {
            get => stream.Position + offset;
            set => Seek(value + offset, SeekOrigin.Begin);
        }

        #endregion Public Properties

        #region Public Methods

        public SubStream(Stream baseStream, long offset, long length, bool leaveOpen)
        {
            stream = baseStream;
            this.offset = offset;
            Length = length;
            this.leaveOpen = leaveOpen;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var maxCount = (int)((this.offset + Length) - Position);
            var actualCount = Math.Min(count, maxCount);

            if (actualCount == 0)
                return 0;

            return stream.Read(buffer, offset, actualCount);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var position = -1L;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset + this.offset;
                    break;

                case SeekOrigin.Current:
                    position = Position + offset;
                    break;

                case SeekOrigin.End:
                    position = this.offset + Length + offset;
                    break;
            }

            if (position < this.offset || position >= this.offset + Length)
                throw new NotSupportedException();

            stream.Seek(position, SeekOrigin.Begin);
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

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !leaveOpen)
                stream.Dispose();
        }

        #endregion Protected Methods
    }
}