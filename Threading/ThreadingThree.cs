﻿using System;
using System.Threading;

namespace ThreadingThree
{
    internal class Test
    {
        public static void Run()
        {
            Console.WriteLine("Main thread id: {0}", Thread.CurrentThread.ManagedThreadId);

            var t = new Thread(Go);
            t.Name = "worker";
            t.IsBackground = true;
            t.Start();
            t.Join();

            Console.WriteLine("End");
        }

        public static void Go()
        {
            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(200);
                Console.WriteLine("Worker {0} ({1}). Step {2}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name, i);
            }
        }
    }
}