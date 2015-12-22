using System;
using System.Collections.Generic;
using System.Threading;
using Threading;

namespace ThreadingWaitPulseWorker
{
    internal class Test
    {
        private static Worker[] pool;

        public static void Run()
        {
            var workersCount = 10;
            Program.WriteLine("Booting up...");

            //get workers pool
            var countdown = new CountdownEvent(workersCount);
            pool = GetPool(countdown);

            //send commands from console until ESC pressed
            string command;
            while ((command = Console.ReadLine()).ToLower() != "quit")
                SendCommand(command);

            Program.WriteLine("End all workers");
            for (var i = 0; i < pool.Length; i++)
                pool[i].End();

            countdown.Wait();
            Program.WriteLine("End...");
        }

        private static Worker[] GetPool(CountdownEvent countdown)
        {
            var count = countdown.InitialCount;
            var workers = new Worker[count];

            for (var i = 0; i < count; i++)
            {
                var worker = workers[i] = new Worker(i + 1, countdown);
                new Thread(worker.Poll).Start();
            }

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

            if (id < 1 || id > pool.Length)
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
                pool[id - 1].End();
            else
                pool[id - 1].Send(new Message(message, duration));
        }
    }

    internal class Worker
    {
        private int id;
        private Queue<Message> messages = new Queue<Message>();
        private object locker = new object();
        private CountdownEvent countdown;

        public Worker(int id, CountdownEvent countdown)
        {
            this.id = id;
            this.countdown = countdown;
        }

        public void Poll()
        {
            //setup thread name
            Thread.CurrentThread.Name = "Worker#" + id;

            while (true)
            {
                Message message;

                //lock until message achieved
                lock (locker)
                {
                    while (messages.Count == 0)
                        Monitor.Wait(locker);

                    message = messages.Dequeue();
                }

                //end if null message
                if (message == null)
                    break;

                Run(message);
            }

            //signal to countdown before end
            countdown.Signal();
        }

        public void Send(Message message)
        {
            //add message to messages queue
            lock (locker)
            {
                messages.Enqueue(message);
                Monitor.Pulse(locker);
            }
        }

        public void End()
        {
            lock (locker)
            {
                messages.Enqueue(null);
                Monitor.Pulse(locker);
            }
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