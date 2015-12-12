using System;
using System.Threading;
using Threading;

namespace ThreadingCancellationToken
{
    internal class Test
    {
        public static void Run()
        {
            Program.WriteLine("Starting program");
            var cancelSource = new CancellationTokenSource(500);
            var worker = new Worker(cancelSource.Token, DoSomeWork, () =>
              {
                  Program.WriteLine("Work cancelled");
              });
            Thread t = new Thread(worker.Work);
            t.Start();
        }

        public static void DoSomeWork(CancellationToken cancelToken)
        {
            while (true)
            {
                Thread.Sleep(100);
                Program.WriteLine("Pending to be cancelled...");
                cancelToken.ThrowIfCancellationRequested();
            }
        }
    }

    internal class Worker
    {
        private Action<CancellationToken> work;
        private Action cleanup;
        private CancellationToken token;

        public Worker(CancellationToken token, Action<CancellationToken> work, Action cleanup = null)
        {
            this.token = token;
            this.work = work;
            this.cleanup = cleanup;
        }

        public void Work()
        {
            try
            {
                work(token);
            }
            catch
            {
                if (cleanup != null)
                    cleanup();
            }
        }
    }
}