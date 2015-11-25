using System;
using System.Collections.Generic;
using System.Threading;
using Threading;

namespace ThreadingProducerConsumerManual
{
    internal class Test
    {
        public static void Run()
        {
            Program.WriteLine("Start");
            Thread.CurrentThread.Name = "Main";
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
        private ManualResetEventSlim gate = new ManualResetEventSlim(false);
        private Thread worker;
        private readonly object locker = new object();
        private Queue<string> tasks = new Queue<string>();

        public ProducerConsumerQueue()
        {
            Program.WriteLine("Create queue");
            worker = new Thread(Work);
            worker.Name = "Worker";
            worker.Start();
        }

        public void AddTask(string task)
        {
            lock (locker)
            {
                Program.WriteLine("Adding task `{0}`", task);
                tasks.Enqueue(task);
            }
            gate.Set();
        }

        public void Dispose()
        {
            // Signal the consumer to exit.
            AddTask(null);

            // Wait for the consumer's thread to finish.
            worker.Join();

            // Release any OS resources.
            gate.Dispose();

            Program.WriteLine("Queue disposed");
        }

        public void Work()
        {
            while (true)
            {
                string task = null;

                //get next task
                lock (locker)
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                        if (task == null)
                        {
                            Program.WriteLine("Termination signal received. Ending");
                            return;
                        }
                    }

                if (task == null)
                {
                    Program.WriteLine("No more tasks, wait for a signal");
                    gate.Wait();
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