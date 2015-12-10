using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using Threading;

namespace ThreadingSynchronizationAttribute
{
    internal class Test
    {
        public static void Run()
        {
            var safe = new AutoLock();
            new Thread(safe.Demo).Start();
            new Thread(safe.Demo).Start();
            safe.Demo();
        }
    }

    [Synchronization(SynchronizationAttribute.REQUIRES_NEW)]
    internal class AutoLock : ContextBoundObject
    {
        public void Demo()
        {
            Program.WriteLine("Start...");
            Thread.Sleep(1000);
            Program.WriteLine("End.");
        }
    }
}