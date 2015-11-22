using System;
using System.Threading;

namespace Threading
{
    internal class TestTwo
    {
        public static void Run()
        {
            var sample = new Sample();
            int order = 0;
            Thread.CurrentThread.Name = "main";

            Console.WriteLine("Main thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            var t = new Thread(sample.Go);
            t.Name = "worker";
            t.IsBackground = true;
            t.Start(++order);

            sample.Go(++order);
            t.Join();

            Console.WriteLine("End");
        }
    }

    internal class Sample
    {
        private readonly object locker = new object();
        private bool done;

        public void Go(object order)
        {
            lock (locker)
            {
                Console.WriteLine("Current thread: id {0}, order {1}, name {2}", Thread.CurrentThread.ManagedThreadId, order, Thread.CurrentThread.Name);
                if (done)
                    Console.WriteLine("Already done");
                else
                {
                    Console.WriteLine("Now done");
                    done = true;
                }
            }
        }
    }
}