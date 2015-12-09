using System.Threading;
using Threading;

namespace ThreadingFive
{
    internal class Test
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(3);

        public static void Run()
        {
            using (var countdown = new CountdownEvent(5))
            {
                Program.WriteLine("Start");
                for (var i = 1; i <= countdown.InitialCount; i++)
                {
                    var t = new Thread(Enter);
                    t.Name = "Worker " + i;
                    Program.WriteLine(t.Name + " starting");
                    t.Start(countdown);
                }
                countdown.Wait();
            }
            Program.WriteLine("End");
        }

        private static void Enter(object countdown)
        {
            var name = Thread.CurrentThread.Name;

            Program.WriteLine(name + " wants to enter");
            semaphore.Wait();

            Program.WriteLine(name + " is in!");
            Thread.Sleep(50);

            Program.WriteLine(name + " is leaving");
            semaphore.Release();

            (countdown as CountdownEvent).Signal();
        }
    }
}