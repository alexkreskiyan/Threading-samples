using System;
using System.Threading;

namespace ThreadingFive
{
    internal class Test
    {
        static SemaphoreSlim semaphore = new SemaphoreSlim(3);

        public static void Run()
        {
            Console.WriteLine("Start");
            for (var i = 1; i <= 5; i++)
            {
                var t = new Thread(Enter);
                t.Start(i);
                t.Join();
            }
            Console.WriteLine("End");
        }

        private static void Enter(object id)
        {
            Console.WriteLine(id + " wants to enter");
            semaphore.Wait();
            Console.WriteLine(id + " is in!");
            Thread.Sleep(500 * (int)id);
            Console.WriteLine(id + " is leaving");
            semaphore.Release();
        }
    }

    public class Instance
    {
        private string name;

        public Instance(string name)
        {
            this.name = name;
        }

        public void Run(object state)
        {
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine("[I{0}:T{1}:{2}]", name, Thread.CurrentThread.ManagedThreadId, i);
            }
        }

        public override string ToString()
        {
            return string.Concat("instance ", name);
        }
    }
}