using OnTheGoPlayer.ViewModels;
using OnTheGoPlayer.Views;
using System.Threading;
using System.Windows;

namespace OnTheGoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private Fields

        private const string APP_GUID = "9d598cdb-8fd5-4828-a544-56a8a5fe4b60";

        private Mutex singleInstanceMutex;

        #endregion Private Fields

        #region Private Methods

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            singleInstanceMutex = new Mutex(true, $"Local\\{APP_GUID}", out var createdNew);
            if (!createdNew)
            {
                Shutdown();
                return;
            }

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var profileManagerViewModel = new ProfileManagerViewModel();
            var result = new ProfileManagerWindow { DataContext = profileManagerViewModel }.ShowDialog() ?? false;
            if (!result)
            {
                Shutdown();
                return;
            }

            var wnd = new MainWindow
            {
                DataContext = new MainViewModel()
                {
                    Database = profileManagerViewModel.SelectedDatabase,
                },
            };

            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = wnd;
            wnd.Show();
        }

        #endregion Private Methods
    }
}