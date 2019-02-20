using Dropbox.Api;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnTheGoPlayer.Dal.MediaMonkeyDropboxDB
{
    /// <summary>
    /// Interaction logic for DropboxAuthDialog.xaml
    /// </summary>
    public partial class DropboxAuthDialog : MetroWindow
    {
        #region Private Fields

        private readonly Regex fragmentRegex = new Regex(@"(?<key>\w+)\=(?<value>[^&]*)");

        #endregion Private Fields

        #region Public Constructors

        public DropboxAuthDialog()
        {
            AuthorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, "zv9bia97vj4w1iw", new Uri("http://localhost/"));

            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Properties

        public Uri AuthorizeUri { get; }

        public OAuth2Response Response { get; private set; }

        #endregion Public Properties

        #region Private Methods

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!e.Uri.IsLoopback)
                return;

            Response = DropboxOAuth2Helper.ParseTokenFragment(e.Uri);

            DialogResult = true;
            Close();
        }

        #endregion Private Methods
    }
}