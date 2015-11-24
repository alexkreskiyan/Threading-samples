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

            new Thread(Waiter).Start();
            Thread.Sleep(1000);

            waitHandle.Set();

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