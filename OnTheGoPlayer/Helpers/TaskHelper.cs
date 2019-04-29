using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Helpers
{
    using System.Diagnostics;

    internal static class TaskHelper
    {
        public static async Task<T> Retry<T>(Func<Task<T>> taskFunc)
        {
            var stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Start();
                try
                {
                    return await taskFunc();
                }
                catch (Exception exception)
                {
                    stopwatch.Stop();
                    Console.WriteLine(exception);
                    Console.WriteLine($"Retrying after {stopwatch.Elapsed}");
                    await Task.Delay(stopwatch.Elapsed);
                }
            }
        }

        public static async Task<T> Retry<T>(Func<CancellationToken, Task<T>> taskFunc, TimeSpan initialWaitTime, CancellationToken cancellationToken)
        {
            var currentWaitTime = initialWaitTime;
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await taskFunc(cancellationToken);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine($"Retrying after {currentWaitTime}");
                    await Task.Delay(currentWaitTime, cancellationToken);
                    currentWaitTime += currentWaitTime;
                }
            }
        }
    }
}