using CSCore;

namespace OnTheGoPlayer.Bl
{
    internal class NullWaveSource : IWaveSource
    {
        #region Private Constructors

        private NullWaveSource()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        public static IWaveSource Instance { get; } = new NullWaveSource();

        public bool CanSeek => true;

        public long Length => 0;

        public long Position { get => 0; set { } }

        public WaveFormat WaveFormat { get; } = new WaveFormat(44000, 8, 2);

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
        }

        public int Read(byte[] buffer, int offset, int count) => 0;

        #endregion Public Methods
    }
}