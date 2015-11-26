using System;
using System.Threading;

namespace Threading
{
    internal class Program
    {
        private static object stdoutLocker = new object();

        private static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";
            ThreadingWaitForSingleObject.Test.Run();
        }

        public static void WriteLine(string message)
        {
            Write(message);
        }

        public static void WriteLine(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        private static void Write(string message)
        {
            lock (stdoutLocker)
            {
                Console.Write(DateTime.Now.ToString("HH:mm:ss.ffff "));
                Console.Write("{0,-15} ", string.Concat('[', Thread.CurrentThread.Name, ']'));
                Console.WriteLine(message);
            }
        }
    }
}