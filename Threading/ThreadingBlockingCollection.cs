using System;
using System.Collections.Concurrent;
using System.Threading;
using Threading;

namespace ThreadingBlockingCollection
{
    internal class Test
    {
        public static void Run()
        {
            Program.WriteLine("Start");

            using (ProducerConsumerQueue queue = new ProducerConsumerQueue())
            {
                queue.AddTask("Hello");

                for (var i = 1; i <= 10; i++)
                    queue.AddTask("Say " + i);

                queue.AddTask("Goodbye!");
            }
            Program.WriteLine("End");
        }
    }

    internal class ProducerConsumerQueue : IDisposable
    {
        private Thread worker;
        private readonly object locker = new object();
        private BlockingCollection<string> tasks = new BlockingCollection<string>();

        public ProducerConsumerQueue()
        {
            Program.WriteLine("Create queue");
            worker = new Thread(Work);
            worker.Name = "Worker";
            worker.Start();
        }

        public void AddTask(string task)
        {
            Program.WriteLine("Adding task `{0}`", task);
            tasks.Add(task);
        }

        public void Dispose()
        {
            // Signal the consumer to exit.
            AddTask(null);

            // Wait for the consumer's thread to finish.
            worker.Join();

            Program.WriteLine("Queue disposed");
        }

        public void Work()
        {
            while (true)
            {
                var task = tasks.Take();

                if (task == null)
                {
                    Program.WriteLine("No more tasks, wait for a signal");
                    return;
                }
                else
                {
                    Program.WriteLine("Performing task: {0}", task);
                    Thread.Sleep(150);
                }
            }
        }
    }
}