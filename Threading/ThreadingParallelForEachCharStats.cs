using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Threading;

namespace ThreadingParallelForEachCharStats
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

            var locker = new object();
            var stats = new int[lettersCount];

            Parallel.ForEach(
                Enumerable.Range(0, 100000000),
                () => new int[lettersCount],
                (i, state, frequences) =>
                {
                    var ch = localRandom.Value.Next(0, lettersCount);
                    ++frequences[ch];
                    return frequences;
                },
                frequences =>
                {
                    lock (locker)
                    {
                        stats = stats.Zip(frequences, (f1, f2) => f1 + f2).ToArray();
                    }
                }
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