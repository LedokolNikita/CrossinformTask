using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace CrossinformTask
{
    class Program
    {
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            var path = @"FileForTests/BigFile.txt";
            var readasync = new ParserFileMultithreaded();
            stopWatch.Start();
            var res = readasync.ReadAllFileAsync(path, 1024*10, tokenSource.Token).
                ContinueWith(task =>
                             {
                                 tokenSource.Cancel();
                                 return task.Result;
                             });

            var cancellationTask = Task.Run(() =>
            {
                Console.ReadKey(true);
                if (res.IsCompleted) return;

                tokenSource.Cancel();
            }, tokenSource.Token);

            Console.WriteLine(res.Result);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }
    }
}
