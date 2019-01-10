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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnTheGoPlayer.Views.Controls
{
    /// <summary>
    /// Interaction logic for ProgressDataBar.xaml
    /// </summary>
    public partial class ProgressDataBar : UserControl
    {
        #region Public Fields

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ProgressData), typeof(ProgressDataBar), new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Constructors

        public ProgressDataBar()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Properties

        public ProgressData Data
        {
            get { return (ProgressData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        #endregion Public Properties
    }
}