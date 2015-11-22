using System;
using System.Threading;

namespace ThreadingFour
{
    internal class Test
    {
        public static void Run()
        {
            Console.WriteLine("Start");
            RunThreadPoolDirectly();
            //Thread.Sleep(100);
            Console.WriteLine("End");
        }

        private static void RunThreadPoolDirectly()
        {
            for (var i = 0; i < 6; i++)
            {
                var instance = new Instance(i.ToString());
                if (ThreadPool.QueueUserWorkItem(instance.Run))
                    Console.WriteLine("{0} run queued", instance);
                else
                    Console.WriteLine("{0} run queue failed", instance);
            }
        }

        private static void RunTaskFactoryThreadPool()
        {
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