using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnTheGoPlayer.Helpers
{
    internal class BindingProxy : DependencyObject
    {
        #region Public Fields

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Properties

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        #endregion Public Properties
    }
}