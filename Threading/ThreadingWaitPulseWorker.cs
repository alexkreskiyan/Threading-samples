using System;
using System.Collections.Generic;
using System.Threading;
using Threading;

namespace ThreadingWaitPulseWorker
{
    internal class Test
    {
        private static Worker[] Pool;

        public static void Run()
        {
            var workersCount = 3;
            var workerThreadsCount = 4;
            Program.WriteLine("Booting up...");

            using (var countdown = new CountdownEvent(workersCount))
                Process(countdown, workerThreadsCount);

            Program.WriteLine("End...");
        }

        private static void Process(CountdownEvent countdown, int workerThreadsCount)
        {
            //get pool
            Pool = GetPool(countdown, workerThreadsCount);

            //send commands from console until ESC pressed
            string command;
            while ((command = Console.ReadLine()).ToLower() != "quit")
                SendCommand(command);

            //send signal to end all workers
            Program.WriteLine("End all workers");
            for (var i = 0; i < Pool.Length; i++)
                Pool[i].End();

            countdown.Wait();
        }

        private static Worker[] GetPool(CountdownEvent countdown, int workerThreadsCount)
        {
            var count = countdown.InitialCount;
            var workers = new Worker[count];

            for (var i = 0; i < count; i++)
                workers[i] = new Worker(i + 1, countdown, workerThreadsCount);

            return workers;
        }

        private static void SendCommand(string command)
        {
            Program.WriteLine("Sending command `{0}`", command);

            var parts = command.Split(" ".ToCharArray(), 3);
            if (parts.Length < 3)
            {
                Program.WriteLine("Invalid command format. Check whether you specified worker, duration and message");
                return;
            }

            int id;
            if (!int.TryParse(parts[0], out id))
            {
                Program.WriteLine("Invalid worker id");
                return;
            }

            if (id < 1 || id > Pool.Length)
            {
                Program.WriteLine("Invalid range of worker id");
                return;
            }

            int duration;
            if (!int.TryParse(parts[1], out duration))
            {
                Program.WriteLine("Invalid command duration duration");
                return;
            }

            var message = parts[2];

            if (message == string.Empty)
                Pool[id - 1].End();
            else
                Pool[id - 1].Send(new Message(message, duration));
        }
    }

    internal class Worker
    {
        private Queue<Message> Messages = new Queue<Message>();
        private object Locker = new object();
        private bool Active = true;

        public Worker(int id, CountdownEvent countdown, int workerThreadsCount)
        {
            //run internal workers in separate thread
            var workerThread = new Thread(() =>
            {
                Program.WriteLine("Worker launched...");
                using (var internalCountdown = new CountdownEvent(workerThreadsCount))
                {
                    for (var i = 1; i <= workerThreadsCount; i++)
                    {
                        //boot new worker thread
                        var thread = new Thread(Poll);
                        thread.Name = string.Concat("SubWorker#", id, ".", i);
                        thread.Start(internalCountdown);
                    }

                    internalCountdown.Wait();
                }

                Program.WriteLine("Worker ending...");

                //signal to outer countdown, that worker ended;
                countdown.Signal();
            });

            workerThread.Name = "Worker#" + id;
            workerThread.Start();
        }

        public void Send(Message message)
        {
            //add message to messages queue
            lock (Locker)
            {
                Messages.Enqueue(message);
                Monitor.Pulse(Locker);
            }
        }

        public void End()
        {
            lock (Locker)
            {
                Active = false;
                Monitor.PulseAll(Locker);
            }
        }

        private void Poll(object countdown)
        {
            Program.WriteLine("SubWorker launched...");

            while (true)
            {
                Program.WriteLine("Waiting for a task...");

                Message message = null;
                lock (Locker)
                {
                    //wait for message if active and there are no messages
                    while (Active && Messages.Count == 0)
                        Monitor.Wait(Locker);

                    //either message received or active state changed
                    if (Messages.Count > 0)
                        message = Messages.Dequeue();
                }

                //here we are either non-active, or we have a message to run with
                if (Active)
                    Run(message);
                else
                    break;
            }

            Program.WriteLine("SubWorker ending...");

            //signal to countdown before end
            (countdown as CountdownEvent).Signal();
        }

        private void Run(Message message)
        {
            Program.WriteLine("Starting `{0}`...", message.Text);

            Thread.Sleep(message.Duration * 1000);

            Program.WriteLine("Finishing with `{0}`...", message.Text);
        }
    }

    internal class Message
    {
        public readonly string Text;
        public readonly int Duration;

        public Message(string text, int duration)
        {
            Text = text;
            Duration = duration;
        }
    }
}