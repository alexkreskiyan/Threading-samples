using System.Threading;
using Threading;

namespace ThreadingInterlockedSimple
{
    internal class Test
    {
        public static void Run()
        {
            using (var countdown = new CountdownEvent(10))
            {
                for (var i = 0; i < 10; i++)
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
        public static int x = 1000;

        public static void Run(object countdown)
        {
            Program.WriteLine("Decrementing...");

            for (var i = 0; i < 100; i++)
                x = x - 1;

            (countdown as CountdownEvent).Signal();
        }
    }
}