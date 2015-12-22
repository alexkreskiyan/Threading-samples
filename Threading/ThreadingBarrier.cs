using System.Threading;
using Threading;

namespace ThreadingBarrier
{
    internal class Test
    {
        private static Barrier Barrier = new Barrier(5);

        public static void Run()
        {
            var words = 5;
            for (var i = 0; i < Barrier.ParticipantCount; i++)
                new Thread(Speak) { Name = "Worker#" + i }.Start(words);
        }

        private static void Speak(object raw)
        {
            int count = (int)raw;
            for (var i = 0; i < count; i++)
            {
                Program.WriteLine(" {0}", i);
                Barrier.SignalAndWait();
            }
        }
    }
}