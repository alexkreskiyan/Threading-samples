using System;

namespace Threading
{
    internal class Program
    {
        static object stdoutLocker = new object();
        private static void Main(string[] args)
        {
            ThreadingAutoResetEvent.Test.Run();
        }

        public static void WriteLine(string message)
        {
            lock (stdoutLocker)
            {
                Console.Write(DateTime.Now.ToString("HH:mm:ss.ffff: "));
                Console.WriteLine(message);
            }
        }
    }
}