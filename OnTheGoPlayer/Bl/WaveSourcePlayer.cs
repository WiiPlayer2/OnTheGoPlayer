using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using NullGuard;
using System;
using System.ComponentModel;

namespace OnTheGoPlayer.Bl
{
    internal class WaveSourcePlayer : INotifyPropertyChanged, IDisposable
    {
        #region Private Fields

        private ISoundOut soundOut;

        #endregion Private Fields

        #region Public Constructors

        public WaveSourcePlayer()
        {
            var mmdEnumerator = new MMDeviceEnumerator();
            soundOut = new WasapiOut
            {
                Device = mmdEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia),
                Latency = 100,
            };
            soundOut.Initialize(NullWaveSource.Instance);
            soundOut.Stopped += SoundOut_Stopped;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CurrentSongEnded;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        [AllowNull]
        public IWaveSource CurrentWaveSource { get; set; }

        public bool IsPlaying { get; private set; }

        public float Volume
        {
            get => soundOut.Volume;
            set => soundOut.Volume = value;
        }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
        }

        public void Pause() => soundOut.Pause();

        public void Play() => soundOut.Play();

        public void Stop()
        {
            lock (this)
            {
                soundOut.Stopped -= SoundOut_Stopped;
                soundOut.Stop();
                soundOut.Stopped += SoundOut_Stopped;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void OnCurrentWaveSourceChanged(IWaveSource oldSource, IWaveSource newSource)
        {
            if (oldSource != null)
            {
                Stop();
            }

            soundOut.Initialize(newSource ?? NullWaveSource.Instance);
        }

        private void OnPropertyChanged(string propertyName, object before, object after)
        {
            switch (propertyName)
            {
                case nameof(CurrentWaveSource):
                    OnCurrentWaveSourceChanged(before as IWaveSource, after as IWaveSource);
                    break;
            }
        }

        private void SoundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            CurrentSongEnded?.Invoke(this, EventArgs.Empty);
        }

        #endregion Private Methods
    }
}