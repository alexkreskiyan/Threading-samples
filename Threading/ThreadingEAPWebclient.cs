using System;
using System.Net;
using System.Threading;
using Threading;

namespace ThreadingEAPWebClient
{
    internal class Test
    {
        public static void Run()
        {
            var wc = new WebClient();
            wc.DownloadDataCompleted += (sender, args) =>
              {
                  if (args.Cancelled)
                      Program.WriteLine("Canceled");
                  else if (args.Error != null)
                      Program.WriteLine("Exception: {0}", args.Error.Message);
                  else
                      Program.WriteLine("{0} chars downloaded", args.Result.Length);

              };
            wc.DownloadDataAsync(new Uri("http://alex-nl.azurewebsites.net/"));
            Thread.Sleep(1000);
        }
    }
}