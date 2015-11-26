using System.Threading;
using Threading;

namespace ThreadingWaitForSingleObject
{
    class Test
    {
        public static void Run()
        {
            var handle = new ManualResetEvent(false);

            Program.WriteLine("Enqueue repeated work");
            var reg = ThreadPool.RegisterWaitForSingleObject(handle, Work, "Some Data", -1, false);
            handle.Set();

            Thread.Sleep(500);

            Program.WriteLine("Unregister handle to stop working");
            reg.Unregister(handle);
        }

        public static void Work(object data, bool timedOut)
        {
            Program.WriteLine("Working with " + data);
            Thread.Sleep(100);
            Program.WriteLine("Finished working with " + data);
        }
    }
}
