using System.ComponentModel;
using System.Threading;
using Threading;

namespace ThreadingBackgroundWorkerSimple
{
    internal class Test
    {
        private static BackgroundWorker worker = new BackgroundWorker();
        private static ManualResetEvent waitHandle = new ManualResetEvent(false);

        public static void Run()
        {
            Program.WriteLine("Add some work to worker");
            worker.DoWork += DoWork;

            Program.WriteLine("Run worker async");
            worker.RunWorkerAsync("some work");
            worker.RunWorkerCompleted += (sender, e) =>
            {
                Program.WriteLine("Job ended with result {0}", e.Result);
                waitHandle.Set();
            };

            Program.WriteLine("Wait for waithandle set to end...");
            waitHandle.WaitOne();
        }

        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            Program.WriteLine("Working with {0}", e.Argument);
            Thread.Sleep(500);
            Program.WriteLine("Working with {0} finished", e.Argument);
        }
    }
}