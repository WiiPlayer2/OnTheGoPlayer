using System.Threading;
using System.Windows;

namespace OnTheGoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string APP_GUID = "9d598cdb-8fd5-4828-a544-56a8a5fe4b60";
        private Mutex singleInstanceMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            singleInstanceMutex = new Mutex(true, $"Local\\{APP_GUID}", out var createdNew);
            if (!createdNew)
                Shutdown();
            else
                base.OnStartup(e);
        }
    }
}