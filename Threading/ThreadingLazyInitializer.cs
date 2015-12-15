using System.Threading;
using Threading;

namespace ThreadingLazyInitializer
{
    internal class Test
    {
        private static ManualResetEventSlim waitHandle = new ManualResetEventSlim(false);
        private static object locker = new object();

        public static void Run()
        {
            Program.WriteLine("Running");

            var worker = new SharedWorker();
            for (var i = 0; i < 10; i++)
            {
                new Thread((id) =>
                {
                    waitHandle.WaitHandle.WaitOne();
                    worker.Work("Worker" + id);
                }).Start(i);
            }

            Program.WriteLine("Setting resource value");
            SharedWorker.Resource.Value = "demo";
            waitHandle.Set();
        }
    }

    internal class SharedWorker
    {
        private static SharedResource resource;

        public static SharedResource Resource
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref resource, () => new SharedResource());

                return resource;
            }
        }

        public void Work(string name)
        {
            Thread.CurrentThread.Name = name;
            Program.WriteLine("Resource value is {0}", Resource.Value);
        }
    }

    internal class SharedResource
    {
        public string Value { get; set; }
    }
}