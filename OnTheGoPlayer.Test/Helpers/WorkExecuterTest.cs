using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using OnTheGoPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Helpers
{
    [TestFixture, Timeout(10000)]
    public class WorkExecuterTest
    {
        #region Public Methods

        [Test]
        public void Execute_WithAnyAction_ShouldReturnImmediately()
        {
            var executer = new WorkExecuter();

            Action act = () => executer.Execute(() => Thread.Sleep(5000));

            act.ExecutionTime().Should().BeLessThan(1.Seconds());
        }

        [Test]
        public async void Execute_WithAnyAction_ShouldSetIsWorking()
        {
            var executer = new WorkExecuter();

            var task = executer.Execute(() => Thread.Sleep(5000));

            executer.IsWorking.Should().BeTrue();

            await task;

            executer.IsWorking.Should().BeFalse();
        }

        [Test]
        public void Execute_WithAnyAsyncAction_ShouldReturnImmediately()
        {
            var executer = new WorkExecuter();

            Action act = () => executer.Execute(async () => await Task.Delay(5000));

            act.ExecutionTime().Should().BeLessThan(1.Seconds());
        }

        #endregion Public Methods
    }
}