using OnTheGoPlayer.Bl;
using OnTheGoPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            PreviousCommand = new Command(Player.Previous);
            NextCommand = new Command(Player.Next);
            PlayCommand = new Command(Player.Play, () => CurrentState != PlayerState.Playing, this);
            PauseCommand = new Command(Player.Pause, () => CurrentState == PlayerState.Playing, this);

            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.DataBind, TimerElapsed, Dispatcher.CurrentDispatcher);
            timer.Start();
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Song CurrentSong => Player.CurrentSong;

        public PlayerState CurrentState => Player.CurrentState;

        public TimeSpan Length => Player.Length;

        public Command NextCommand { get; }

        public Command PauseCommand { get; }

        public Command PlayCommand { get; }

        public Player Player { get; }

        public TimeSpan Position
        {
            get => Player.Position;
            set => Player.Position = value;
        }

        public Command PreviousCommand { get; }

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
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            InvokePropertyChanged(nameof(Position));
        }

        #endregion Private Methods
    }
}