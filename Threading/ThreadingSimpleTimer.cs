using System;
using System.Threading;
using Threading;

namespace ThreadingSimpleTimer
{
    internal class Test
    {
        public static void Run()
        {
            Program.WriteLine("Running");
            var timer = new Timer((data) => Program.WriteLine("tick..."), null, 500, 500);
            Console.ReadLine();
            timer.Dispose();
        }
    }
}