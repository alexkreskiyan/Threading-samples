﻿using System;
using System.Threading;

namespace ThreadingOne
{
    internal class Test
    {
        public static void Run()
        {
            Thread t = new Thread(WriteY);
            t.Start();
            while (true)
                Console.Write("x");
        }

        public static void WriteY()
        {
            while (true)
                Console.Write("y");
        }
    }
}