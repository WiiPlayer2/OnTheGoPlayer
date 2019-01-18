using NullGuard;
using System;
using System.Windows;
using System.Windows.Data;

namespace OnTheGoPlayer.Helpers
{
    public class StaticResourcePath : StaticResourceExtension
    {
        #region Public Properties

        public PropertyPath Path
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        [return: AllowNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // First get the value from StaticResource
            object o = base.ProvideValue(serviceProvider);
            var ret = (Path == null ? o : PathEvaluator.Evaluate(o, Path));
            return ret;
        }

        #endregion Public Methods

        #region Private Classes

        private class PathEvaluator : DependencyObject
        {
            #region Private Fields

            /// <summary>
            /// This dummy will hold the end result.
            /// </summary>
            private static readonly DependencyProperty DummyProperty =
                DependencyProperty.Register("Dummy", typeof(object),
                typeof(PathEvaluator), new UIPropertyMetadata(null));

            #endregion Private Fields

            #region Public Methods

            public static object Evaluate(object source, PropertyPath path)
            {
                PathEvaluator d = new PathEvaluator();
                BindingOperations.SetBinding(d, DummyProperty, new Binding(path.Path) { Source = source });

                // Force binding to give us the desired value defined in path.
                var result = d.GetValue(DummyProperty);

                // Clear the binding to leave nice memory footprints
                BindingOperations.ClearBinding(d, DummyProperty);

                return result;
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}