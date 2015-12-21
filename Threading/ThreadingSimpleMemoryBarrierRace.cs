using System;
using System.Threading;

namespace ThreadingSimpleMemoryBarrierRace
{
    internal class Test
    {
        private static bool stop = false;

        public static void Run()
        {
            var t = new Thread(() =>
            {
                Console.WriteLine("thread begin");
                bool toggle = false;
                while (!stop)
                {
                    Thread.MemoryBarrier();
                    toggle = !toggle;
                }
                Console.WriteLine("thread end");
            });
            t.Start();
            Thread.Sleep(500);
            stop = true;
            Console.WriteLine("stop = true");
            Console.WriteLine("waiting...");
            t.Join();
        }
    }
}