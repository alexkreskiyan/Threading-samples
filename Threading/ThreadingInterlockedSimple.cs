using System.Threading;
using Threading;

namespace ThreadingInterlockedSimple
{
    internal class Test
    {
        public static void Run()
        {
            Program.WriteLine("Run parallel...");
            using (var countdown = new CountdownEvent(100))
            {
                for (var i = 0; i < countdown.InitialCount; i++)
                {
                    var t = new Thread(Incremental.Run);
                    t.Name = "Worker" + i;
                    t.Start(countdown);
                }

                countdown.Wait();
            }

            Program.WriteLine("Decremented to {0}", Incremental.x);
        }
    }

    internal class Incremental
    {
        public static int x = 200000;

        public static void Run(object countdown)
        {
            for (var i = 0; i < 2000; i++)
                x = x - 1;

            (countdown as CountdownEvent).Signal();
        }
    }
}