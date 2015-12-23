using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Threading;

namespace ThreadingPLINQCharStats
{
    internal class Test
    {
        public static void Run()
        {
            var localRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
            var lettersCount = 'z' - 'a' + 1;

            Program.WriteLine("Start...");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var stats = Enumerable.Range(0, 100000000)
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select((i) => localRandom.Value.Next(0, lettersCount))
                .Aggregate(
                    () => new int[lettersCount],
                    (frequences, ch) => { ++frequences[ch]; return frequences; },
                    (main, local) => main.Zip(local, (f1, f2) => f1 + f2).ToArray(),
                    //(main, local) => main,
                    result => result
                );

            var count = stats.Count();
            stopwatch.Stop();

            for (var ch = 0; ch < stats.Count(); ch++)
                Program.WriteLine("{0}: {1}", (char)(ch + 'a'), stats[ch]);

            Program.WriteLine("Total: {0}, elapsed {1}", stats.Sum(), stopwatch.ElapsedMilliseconds);
            Program.WriteLine("End...");
        }
    }
}