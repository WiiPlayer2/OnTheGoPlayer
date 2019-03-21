using MahApps.Metro.Controls;
using NullGuard;
using OnTheGoPlayer.Dal;
using OnTheGoPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnTheGoPlayer.Views
{
    /// <summary>
    /// Interaction logic for ProfileManagerDatabaseSelectDialog.xaml
    /// </summary>
    public partial class ProfileManagerDatabaseSelectDialog : MetroWindow, INotifyPropertyChanged
    {
        #region Public Constructors

        public ProfileManagerDatabaseSelectDialog()
        {
            InitializeComponent();
        }

        public ProfileManagerDatabaseSelectDialog(IEnumerable<IMediaDatabase> databases)
        {
            Databases = databases;
            SelectCommand = new Command(SelectDatabase, () => SelectedDatabase != null, this);

            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<IMediaDatabase> Databases { get; }

        public Command SelectCommand { get; }

        [AllowNull]
        public IMediaDatabase SelectedDatabase { get; set; }

        #endregion Public Properties

        #region Private Methods

        private void SelectDatabase()
        {
            DialogResult = true;
            Close();
        }

        #endregion Private Methods
    }
}