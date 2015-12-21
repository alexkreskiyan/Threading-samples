using System.Collections.Generic;
using System.Threading;
using Threading;

namespace ThreadingInterlockedMonitor
{
    internal class Test
    {
        private static object locker = new object();
        public static void Run()
        {
            Program.WriteLine("Run sequentially...");
            using (var countdown = new CountdownEvent(50))
            {
                for (var i = 0; i < countdown.InitialCount; i++)
                {
                    var t = new Thread(Log);
                    t.Name = "Worker" + i;
                    t.Start(countdown);
                }
                countdown.Wait();
            }

            Program.WriteLine("Race finished");
        }

        private static void Log(object countdown)
        {
            Program.WriteLine("Entered Log");
            lock (locker)
            {
                for (var i = 0; i < 5; i++)
                    Program.WriteLine("{0}...", i);
                (countdown as CountdownEvent).Signal();
            };
            Program.WriteLine("Ended Log");
        }
    }

    internal class Monitored
    {
        private static List<object> locks = new List<object>();

        public static void Enter(object obj)
        {
            while (locks.Contains(obj)) ;
            locks.Add(obj);
            Program.WriteLine("Lock obtained");
        }

        public static void Leave(object obj)
        {
            locks.Remove(obj);
            Program.WriteLine("Lock released");
        }
    }
}