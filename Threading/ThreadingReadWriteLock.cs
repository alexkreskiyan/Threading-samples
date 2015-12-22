using System;
using System.Collections.Generic;
using System.Threading;
using Threading;

namespace ThreadingReadWriteLock
{
    internal class Test
    {
        private static Random Random = new Random();
        private static List<int> List = new List<int>();
        private static ReaderWriterLockSlim ListLock = new ReaderWriterLockSlim();

        public static void Run()
        {
            Program.WriteLine("Start...");

            for (var i = 0; i < 5; i++)
                new Thread(Enumerate) { Name = "Listener#" + i }.Start();

            for (var i = 0; i < 5; i++)
                new Thread(Append) { Name = "Writer#" + i }.Start();

            Thread.Sleep(1000);
            Program.WriteLine("End...");
        }

        private static void Enumerate()
        {
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(50);
                ListLock.EnterReadLock();
                Program.WriteLine("Enumerating...");
                foreach (var num in List)
                    Program.WriteLine("{0}", num);
                ListLock.ExitReadLock();
            }
        }

        private static void Append()
        {
            for (var i = 0; i < 3; i++)
            {
                Thread.Sleep(70);
                ListLock.EnterWriteLock();
                Program.WriteLine("Adding...");
                List.Add(Random.Next(0, 10));
                ListLock.ExitWriteLock();
            }
        }
    }
}