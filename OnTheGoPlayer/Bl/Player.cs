using CSCore;
using CSCore.Codecs.AAC;
using CSCore.Codecs.FLAC;
using CSCore.Codecs.MP3;
using CSCore.Codecs.OGG;
using CSCore.Codecs.WAV;
using NullGuard;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Helpers;
using OnTheGoPlayer.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Bl
{
    public enum RepeatMode
    {
        Off,

        RepeatAll,

        RepeatOne,
    }

    internal enum PlayerState
    {
        Stopped,

        Playing,

        Paused,
    }

    [Janitor.SkipWeaving]
    internal class Player : INotifyPropertyChanged, IDisposable
    {
        #region Private Fields

        private static Random random = new Random();

        private CancellationTokenSource currentCancellationTokenSource;

        private IPlaylistContainer playlistContainer;

        private WaveSourcePlayer waveSourcePlayer;

        #endregion Private Fields

        #region Public Constructors

        public Player()
        {
            waveSourcePlayer = new WaveSourcePlayer();
            waveSourcePlayer.CurrentSongEnded += WaveSourcePlayer_CurrentSongEnded;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Playlist CurrentPlaylist => playlistContainer?.Playlist;

        public RepeatMode CurrentRepeatMode { get; set; }

        [AllowNull]
        public Song CurrentSong { get; private set; }

        public PlayerState CurrentState { get; private set; }

        public bool IsLoading { get; private set; }

        public bool IsShuffleEnabled { get; set; }

        public TimeSpan Length => waveSourcePlayer.CurrentWaveSource?.GetLength() ?? TimeSpan.Zero;

        public TimeSpan Position
        {
            get => waveSourcePlayer.CurrentWaveSource?.GetPosition() ?? TimeSpan.Zero;
            set => waveSourcePlayer.CurrentWaveSource?.SetPosition(value);
        }

        public float Volume { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
            waveSourcePlayer?.Dispose();
        }

        public void Next()
        {
            var songs = playlistContainer.Playlist.Songs.ToList();

            var nextIndex = songs.IndexOf(CurrentSong) + 1;
            if (IsShuffleEnabled)
                nextIndex = random.Next(songs.Count);
            var nextSong = songs[nextIndex % songs.Count];
            Play(nextSong);
        }

        public void Pause()
        {
            waveSourcePlayer.Pause();
            CurrentState = PlayerState.Paused;
        }

        public void Play()
        {
            waveSourcePlayer.Play();
            CurrentState = PlayerState.Playing;
        }

        public void Play(Song song)
        {
            CancellationToken token;
            lock (this)
            {
                currentCancellationTokenSource?.Cancel();
                currentCancellationTokenSource = new CancellationTokenSource();
                token = currentCancellationTokenSource.Token;
            }

            IsLoading = true;
            CurrentSong = song;
            Stop();

            Task.Run(
                () => GetWaveSource(song, token).ContinueWith(
                    task =>
                    {
                        token.ThrowIfCancellationRequested();
                        Play(task.Result);
                    },
                    token,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default),
                token);
        }

        public void Play(IWaveSource waveSource)
        {
            waveSourcePlayer.CurrentWaveSource = waveSource;
            waveSourcePlayer.Volume = Volume;
            Position = TimeSpan.Zero;
            this.InvokePropertyChanged(PropertyChanged, nameof(Length));
            Play();
            IsLoading = false;
        }

        public void Previous()
        {
            var songs = playlistContainer.Playlist.Songs.ToList();
            var nextIndex = songs.IndexOf(CurrentSong) - 1;
            if (nextIndex < 0)
                nextIndex = songs.Count - 1;
            var nextSong = songs[nextIndex % songs.Count];
            Play(nextSong);
        }

        public void SetPlaylistContainer(IPlaylistContainer playlistContainer)
        {
            Stop();
            CurrentSong = null;
            this.playlistContainer = playlistContainer;
        }

        public void Stop()
        {
            waveSourcePlayer.Stop();
            CurrentState = PlayerState.Stopped;
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<IWaveSource> GetWaveSource(Song song, CancellationToken token)
        {
            var stream = await playlistContainer.GetSongStream(song, token);

            switch (song.FileFormat)
            {
                case "m4a":
                    return new AacDecoder(stream);

                case "mp3":
                    return new DmoMp3Decoder(stream);

                case "flac":
                    return new FlacFile(stream);

                case "ogg":
                    return new OggSource(stream).ToWaveSource();

                case "wav":
                    return new WaveFileReader(stream);

                default:
                    Debug.Fail($"Format {song.FileFormat} not supported.");
                    return NullWaveSource.Instance;
            }
        }

        private void OnVolumeChanged()
        {
            waveSourcePlayer.Volume = Volume;
        }

        private void WaveSourcePlayer_CurrentSongEnded(object sender, EventArgs e)
        {
            if (CurrentSong != null)
                SongInfoDB.Instance.IncreaseCounter(CurrentSong);

            if (CurrentRepeatMode == RepeatMode.RepeatOne)
                Play(CurrentSong);
            else
                Next();
        }

        #endregion Private Methods
    }
}