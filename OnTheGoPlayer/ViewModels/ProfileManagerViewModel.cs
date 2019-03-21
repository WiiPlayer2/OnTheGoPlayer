using NullGuard;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.Dal.MediaMonkeyCOM;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using OnTheGoPlayer.Dal.MediaMonkeyDropboxDB;
using OnTheGoPlayer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OnTheGoPlayer.ViewModels
{
    internal class ProfileManagerViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        #endregion Private Fields

        #region Public Constructors

        public ProfileManagerViewModel()
        {
            AddCommand = new Command(AddProfile);
            RemoveCommand = new Command(RemoveProfile, () => SelectedProfile != null, this);
            SelectCommand = new Command<Window>(SelectProfile, _ => SelectedProfile != null, this);

            ProfileRepository.Instance.GetAll().ToList().ForEach(Profiles.Add);
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public Command AddCommand { get; }

        public IEnumerable<IMediaDatabase> Databases { get; } = new IMediaDatabase[]
                {
                    new MMComPlaylistContainerExporter(),
                    new MMDBPlaylistContainerExporter(),
                    new MMDropboxDBMediaDatabase(),
                };

        public bool IsOpening { get; private set; }

        public ObservableCollection<Profile> Profiles { get; } = new ObservableCollection<Profile>();

        public Command RemoveCommand { get; }

        public Command<Window> SelectCommand { get; }

        public IMediaDatabase SelectedDatabase { get; private set; }

        [AllowNull]
        public Profile SelectedProfile { get; set; }

        #endregion Public Properties

        #region Private Methods

        private async void AddProfile()
        {
            var dialog = new ProfileManagerDatabaseSelectDialog(Databases);
            var result = dialog.ShowDialog() ?? false;
            if (!result)
                return;

            var profile = await dialog.SelectedDatabase.TryRegister();
            if (profile.IsSome)
                await dispatcher.InvokeAsync(() =>
                {
                    var actualProfile = profile.GetValueOrDefault();
                    Profiles.Add(actualProfile);
                    ProfileRepository.Instance.Add(actualProfile);
                    SelectedProfile = actualProfile;
                });
        }

        private void RemoveProfile()
        {
            Profiles.Remove(SelectedProfile);
            ProfileRepository.Instance.Remove(SelectedProfile);
        }

        private async void SelectProfile(Window window)
        {
            IsOpening = true;

            var database = Databases.First(o => o.ID == SelectedProfile.InterfaceID);
            await database.Open(SelectedProfile.ProfileData);
            SelectedDatabase = database;

            await dispatcher.InvokeAsync(() =>
            {
                window.DialogResult = true;
                window.Close();
            });
        }

        #endregion Private Methods
    }
}