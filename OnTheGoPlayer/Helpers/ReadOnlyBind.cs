using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace OnTheGoPlayer.Helpers
{
    public static class ReadOnlyBind
    {
        #region Public Fields

        // Using a DependencyProperty as the backing store for OnIsVisibleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnIsVisibleCommandProperty =
            DependencyProperty.RegisterAttached("OnIsVisibleCommand", typeof(ICommand), typeof(ReadOnlyBind), new PropertyMetadata(null, OnIsVisibleCommandChanged));

        #endregion Public Fields

        #region Public Methods

        public static ICommand GetOnIsVisibleCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OnIsVisibleCommandProperty);
        }

        public static void SetOnIsVisibleCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OnIsVisibleCommandProperty, value);
        }

        #endregion Public Methods

        #region Private Methods

        private static void OnIsVisibleCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement uiElement && e.NewValue is ICommand command))
                return;

            uiElement.IsVisibleChanged += (o, args) => command.Execute(o);
            command.Execute(uiElement);
        }

        #endregion Private Methods
    }
}