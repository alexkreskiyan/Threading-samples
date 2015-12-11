using System;
using System.ComponentModel;
using System.Threading;
using Threading;

namespace ThreadingBackgroundWorkerComplex
{
    internal class Test
    {
        private static BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        private static ManualResetEvent waitHandle = new ManualResetEvent(false);

        public static void Run()
        {
            Program.WriteLine("Add some work to worker");
            worker.DoWork += DoWork;

            Program.WriteLine("Subscribe to worker updated");
            worker.ProgressChanged += HandleProgressChanged;
            worker.RunWorkerCompleted += HandleRunWorkerCompleted;


            Program.WriteLine("Run worker async");
            worker.RunWorkerAsync("some work");

            //run a parallel task to cancel worker
            new Thread(() =>
            {
                Console.ReadLine();
                worker.CancelAsync();
            })
            {
                IsBackground = true
            }.Start();

            Program.WriteLine("Wait for waithandle set to end...");
            waitHandle.WaitOne();
        }

        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            Program.WriteLine("Working with {0}", e.Argument);
            for (var i = 0; i <= 100; i += 10)
            {
                Thread.Sleep(100);
                worker.ReportProgress(i, string.Concat(i, '%'));
                if (worker.CancellationPending)
                {
                    Program.WriteLine("Working {0} cancel requested at progress {1}", e.Argument, i);
                    e.Cancel = true;
                    return;
                }
            }
            Program.WriteLine("Working with {0} finished", e.Argument);
        }

        private static void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Program.WriteLine("Work progressed to {0} ({1})", e.ProgressPercentage, e.UserState);
        }

        private static void HandleRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                Program.WriteLine("Work cancelled");
            else if (e.Error != null)
                Program.WriteLine("Work failed with '{0}'", e.Error.Message);
            else
                Program.WriteLine("Work completed with '{0}'", e.Result);

            waitHandle.Set();
        }
    }
}