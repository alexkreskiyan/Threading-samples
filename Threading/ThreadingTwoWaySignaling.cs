using System.Threading;
using Threading;

namespace ThreadingTwoWaySignaling
{
    internal class Test
    {
        private static EventWaitHandle worker = new AutoResetEvent(false);
        private static EventWaitHandle main = new AutoResetEvent(false);
        private static readonly object locker = new object();
        private static string message;

        private static EventWaitHandle waitHandle = new AutoResetEvent(false);

        public static void Run()
        {
            Program.WriteLine("Start");

            new Thread(Work) { Name = "Worker" }.Start();

            worker.WaitOne();                           //wait worker
            lock (locker) message = "aaa";
            Program.WriteLine("{0} set message: {1}", Thread.CurrentThread.Name, message);
            main.Set();                                 //tell worker to go

            worker.WaitOne();
            lock (locker) message = "bbb";               //give another message to worker
            Program.WriteLine("{0} set message: {1}", Thread.CurrentThread.Name, message);
            main.Set();

            worker.WaitOne();
            lock (locker) message = null;               //signal worker to exit
            main.Set();

            worker.WaitOne();
            Program.WriteLine("End");
        }

        private static void Work()
        {
            while (true)
            {
                worker.Set();
                main.WaitOne();
                lock (locker)
                {
                    if (message == null)
                    {
                        Program.WriteLine("Worker received signal to exit");
                        worker.Set();
                        return;
                    }

                    Program.WriteLine("Received: {0}", message);
                }
            }
        }
    }
}