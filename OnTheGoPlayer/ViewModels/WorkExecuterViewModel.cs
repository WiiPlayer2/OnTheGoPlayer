using OnTheGoPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.ViewModels
{
    [Janitor.SkipWeaving]
    internal class WorkExecuterViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Public Constructors

        public WorkExecuterViewModel(WorkExecuter workExecuter)
        {
            WorkExecuter = workExecuter;
            workExecuter.PropertyChanged += WorkExecuter_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public bool ShouldShow { get; private set; }

        public bool ShowError => !WorkExecuter.IsWorking && WorkExecuter.HasError;

        public bool ShowOkay => !WorkExecuter.IsWorking && !WorkExecuter.HasError;

        public WorkExecuter WorkExecuter { get; }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
            WorkExecuter.PropertyChanged -= WorkExecuter_PropertyChanged;
        }

        #endregion Public Methods

        #region Private Methods

        private void WorkExecuter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(WorkExecuter.IsWorking))
                return;

            this.MapPropertyChanged(e, PropertyChanged, nameof(WorkExecuter.IsWorking), nameof(ShowError), nameof(ShowOkay));
            this.MapPropertyChanged(e, PropertyChanged, nameof(WorkExecuter.HasError), nameof(ShowError), nameof(ShowOkay));

            if (WorkExecuter.IsWorking)
                ShouldShow = true;
            else
                Task.Delay(3000).ContinueWith(_ => ShouldShow = false);
        }

        #endregion Private Methods
    }
}