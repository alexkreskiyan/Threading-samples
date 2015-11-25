using System;
using System.Threading;

namespace ThreadingAutoResetEvent
{
    internal class Test
    {
        private static EventWaitHandle waitHandle = new AutoResetEvent(false);

        public static void Run()
        {
            Console.WriteLine("Start");

            var t = new Thread(Waiter);
            t.Start();
            Thread.Sleep(1000);

            waitHandle.Set();
            t.Join();
            Console.WriteLine("End");
        }

        private static void Waiter()
        {
            Console.WriteLine("Waitiing...");
            waitHandle.WaitOne();
            Console.WriteLine("Notified");
        }
    }
}