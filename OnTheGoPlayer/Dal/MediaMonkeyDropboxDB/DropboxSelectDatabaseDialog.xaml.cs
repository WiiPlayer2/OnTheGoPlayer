using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Dropbox.Api;
using MahApps.Metro.Controls;
using NullGuard;
using OnTheGoPlayer.Dal.MediaMonkeyDB;
using OnTheGoPlayer.ViewModels;
using IOPath = System.IO.Path;

namespace OnTheGoPlayer.Dal.MediaMonkeyDropboxDB
{
    using Dropbox.Api.Files;

    public enum ProgressState
    {
        Progressing,

        Done,

        Failed
    }

    /// <summary>
    /// Interaction logic for DropboxSelectDatabaseDialog.xaml
    /// </summary>
    public partial class DropboxSelectDatabaseDialog : MetroWindow, INotifyPropertyChanged
    {
        #region Private Fields

        private readonly DropboxClient client;

        #endregion Private Fields

        #region Public Constructors

        public DropboxSelectDatabaseDialog()
        {
            InitializeComponent();
        }

        public DropboxSelectDatabaseDialog(DropboxClient dropboxClient)
        {
            client = dropboxClient;
            SubmitCommand = new Command(Submit, () => SelectedSearchMatch?.Metadata.IsFile ?? false && SelectedSearchMatch.Metadata.IsFile, this);

            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public string CurrentHint { get; private set; } = string.Empty;

        public ProgressState CurrentState { get; private set; } = ProgressState.Progressing;

        public MMDBPlaylistContainerExporter MediaDatabase { get; private set; }

        public string LocalDatabasePath { get; private set; }

        [AllowNull]
        public SearchResult SearchResult { get; private set; }

        [AllowNull]
        public SearchMatch SelectedSearchMatch { get; set; }

        public Command SubmitCommand { get; }

        #endregion Public Properties

        #region Private Methods

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            CurrentHint = "Searching for \"mm.db\"...";

            client.Files.SearchAsync("", "mm.db")
                .ContinueWith(task =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (task.IsFaulted)
                        {
                            SetFailed(task.Exception);
                            return;
                        }

                        CurrentHint = string.Empty;
                        CurrentState = ProgressState.Done;
                        SearchResult = task.Result;
                        SelectedSearchMatch = null;
                    });
                });
        }

        private void SetFailed(Exception exception)
        {
            CurrentHint = $"{exception.GetType().Name}: {exception.Message}";
            CurrentState = ProgressState.Failed;
        }

        private async void Submit()
        {
            var metadata = SelectedSearchMatch.Metadata.AsFile;
            CurrentHint = $"Opening \"{metadata.PathDisplay}\"...";
            CurrentState = ProgressState.Progressing;

            try
            {
                var downloadResponse = await client.Files.DownloadAsync(metadata.PathLower);

                var stream = await downloadResponse.GetContentAsStreamAsync();
                var tmpPath = IOPath.GetTempFileName();
                using (var fileStream = File.OpenWrite(tmpPath))
                {
                    await stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }

                try
                {
                    var database = new MMDBPlaylistContainerExporter();
                    await database.Open(tmpPath);
                    await database.Close();
                    LocalDatabasePath = tmpPath;
                    MediaDatabase = database;

                    Dispatcher.Invoke(() =>
                    {
                        DialogResult = true;
                        Close();
                    });
                }
                catch
                {
                    Search();
                }
            }
            catch (Exception e)
            {
                SetFailed(e);
            }
        }

        #endregion Private Methods
    }
}