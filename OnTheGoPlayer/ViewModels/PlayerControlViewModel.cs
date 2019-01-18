using OnTheGoPlayer.Bl;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace OnTheGoPlayer.ViewModels
{
    internal class PlayerControlViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly DispatcherTimer timer;

        #endregion Private Fields

        #region Public Constructors

        public PlayerControlViewModel()
        {
            Player = new Player();
            Player.PropertyChanged += Player_PropertyChanged;

            PreviousCommand = new Command(Player.Previous, true);
            NextCommand = new Command(Player.Next, true);
            PlayCommand = new Command(Player.Play, () => CurrentState != PlayerState.Playing, this);
            PauseCommand = new Command(Player.Pause, () => CurrentState == PlayerState.Playing, this);
            PlayPauseCommand = new Command(PlayPause);
            RepeatCycleCommand = new Command<ToggleButton>(RepeatCycle);

            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.DataBind, TimerElapsed, Dispatcher.CurrentDispatcher);
            timer.Start();
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public RepeatMode CurrentRepeatMode
        {
            get => Player.CurrentRepeatMode;
            set => Player.CurrentRepeatMode = value;
        }

        public Song CurrentSong => Player.CurrentSong;

        public PlayerState CurrentState => Player.CurrentState;

        public bool IsShuffleEnabled
        {
            get => Player.IsShuffleEnabled;
            set => Player.IsShuffleEnabled = value;
        }

        public TimeSpan Length => Player.Length;

        public Command NextCommand { get; }

        public Command PauseCommand { get; }

        public Command PlayCommand { get; }

        public Player Player { get; }

        public Command PlayPauseCommand { get; }

        public TimeSpan Position
        {
            get => Player.Position;
            set => Player.Position = value;
        }

        public Command PreviousCommand { get; }

        public Command<ToggleButton> RepeatCycleCommand { get; }

        public float Volume
        {
            get => Player.Volume;
            set => Player.Volume = value;
        }

        #endregion Public Properties

        #region Private Methods

        private void InvokePropertyChanged(string name)
        {
            //Debug.WriteLine($"InvokePropertyChanged({name})");
            dispatcher.InvokeAsync(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        private void MapProperty(PropertyChangedEventArgs eventArgs, string sourceProp, string targetProp)
        {
            if (eventArgs.PropertyName == sourceProp)
                InvokePropertyChanged(targetProp);
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"Player_PropertyChanged({e.PropertyName})");
            MapProperty(e, nameof(Player.CurrentSong), nameof(Position));
            MapProperty(e, nameof(Player.CurrentSong), nameof(Length));
            MapProperty(e, nameof(Player.CurrentSong), nameof(CurrentSong));
            MapProperty(e, nameof(Player.CurrentState), nameof(CurrentState));
            MapProperty(e, nameof(Player.IsShuffleEnabled), nameof(IsShuffleEnabled));
            MapProperty(e, nameof(Player.CurrentRepeatMode), nameof(CurrentRepeatMode));
        }

        private void PlayPause()
        {
            if (CurrentState != PlayerState.Playing)
                Player.Play();
            else
                Player.Pause();
        }

        private void RepeatCycle(ToggleButton obj)
        {
            CurrentRepeatMode = (obj.IsChecked ?? false) ? RepeatMode.RepeatOne : RepeatMode.RepeatAll;
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            InvokePropertyChanged(nameof(Position));
        }

        #endregion Private Methods
    }
}