using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.FLAC;
using CSCore.Codecs.MP3;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using NullGuard;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Bl
{
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

        private SongInfoDB db = new SongInfoDB();

        private IPlaylistContainer playlistContainer;

        private ISoundOut soundOut;

        private IWaveSource waveSource;

        #endregion Private Fields

        #region Public Constructors

        public Player()
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

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Playlist CurrentPlaylist => playlistContainer?.Playlist;

        [AllowNull]
        public Song CurrentSong { get; private set; }

        public PlayerState CurrentState { get; private set; }

        public TimeSpan Length => waveSource?.GetLength() ?? TimeSpan.Zero;

        public TimeSpan Position
        {
            get => waveSource?.GetPosition() ?? TimeSpan.Zero;
            set => waveSource?.SetPosition(value);
        }

        public float Volume { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
            soundOut.Stopped -= SoundOut_Stopped;
            soundOut?.Dispose();
            waveSource?.Dispose();

            //soundOut = null;
            //waveSource = null;
        }

        public void Next()
        {
            var songs = playlistContainer.Playlist.Songs.ToList();
            var index = songs.IndexOf(CurrentSong);
            var nextSong = songs[(index + 1) % songs.Count];
            Play(nextSong);
        }

        public void Pause()
        {
            soundOut.Pause();
            CurrentState = PlayerState.Paused;
        }

        public void Play()
        {
            soundOut.Play();
            CurrentState = PlayerState.Playing;
        }

        public async void Play(Song song)
        {
            Stop();
            waveSource = await GetWaveSource(song);
            soundOut.Initialize(waveSource);
            soundOut.Volume = Volume;
            Position = TimeSpan.Zero;
            CurrentSong = song;
            Play();
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
            lock (soundOut)
            {
                soundOut.Stopped -= SoundOut_Stopped;
                soundOut.Stop();
                soundOut.Stopped += SoundOut_Stopped;
                CurrentState = PlayerState.Stopped;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<IWaveSource> GetWaveSource(Song song)
        {
            var stream = await playlistContainer.GetSongStream(song);

            switch (song.FileFormat)
            {
                case "mp3":
                    return new DmoMp3Decoder(stream);

                case "flac":
                    return new FlacFile(stream);

                default:
                    Debug.Fail($"Format {song.FileFormat} not supported.");
                    return NullWaveSource.Instance;
            }
        }

        private void OnVolumeChanged()
        {
            soundOut.Volume = Volume;
        }

        private void SoundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (CurrentSong != null)
                db.IncreaseCounter(CurrentSong);
            Next();
        }

        #endregion Private Methods
    }
}