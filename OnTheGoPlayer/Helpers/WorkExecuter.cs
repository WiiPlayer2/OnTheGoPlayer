using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    public class WorkExecuter : INotifyPropertyChanged
    {
        #region Public Constructors

        public WorkExecuter()
        {
            throw new NotImplementedException();
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public bool IsWorking { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public Task Execute(Action action, Action<Exception> onException = null)
        {
            throw new NotImplementedException();
        }

        public Task Execute(Func<Task> asyncAction, Action<Exception> onException = null)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}