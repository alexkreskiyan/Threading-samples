using System.Threading;
using Threading;

namespace ThreadingCountdownEvent
{
    internal class Test
    {
        public static void Run()
        {
            Program.WriteLine("Start");
            var works = new string[]{
                "one", "two", "three", "four", "five",
                "six", "seven", "eight", "nine", "ten"
            };

            using (var countdown = new CountdownEvent(works.Length))
            {
                for (var i = 0; i < works.Length; i++)
                    new Thread(Work) { Name = "Worker" + (i + 1) }.Start(new Task(works[i], countdown));

                countdown.Wait();
            }

            Program.WriteLine("End");
        }

        private static void Work(object data)
        {
            var task = data as Task;

            //emulate work process
            Program.WriteLine("Working on {0}", task.Name);
            //Thread.Sleep(100);
            Program.WriteLine("Finished with {0}", task.Name);

            //signal to countdown
            task.Countdown.Signal();
        }
    }

    internal class Task
    {
        public readonly string Name;
        public readonly CountdownEvent Countdown;

        public Task(string name, CountdownEvent countdown)
        {
            Name = name;
            Countdown = countdown;
        }
    }
}