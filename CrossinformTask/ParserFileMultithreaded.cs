using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace CrossinformTask
{
    public class ParserFileMultithreaded
    {
        private TextParser textParser;

        public ParserFileMultithreaded()
        {
            textParser = new TextParser();
        }
        public async Task<string> ReadAllFileAsync(string filename, int bufferSize, CancellationToken cancellationToken)
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                using var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, buffer.Length, true);
                var queuedTasks = new List<Task>();
                var addToNextString = "";
                while (await file.ReadAsync(buffer, 0, buffer.Length, cancellationToken) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var decodedString = addToNextString + Encoding.Default.GetString(buffer);
                    int indexDivisionforTasks = SearchDivisionIndex(decodedString, decodedString.Length / 2);
                    int indexDivisionforBuffer = SearchDivisionIndex(decodedString, decodedString.Length);

                    if (indexDivisionforTasks != -1 || indexDivisionforBuffer != -1)
                    {
                        addToNextString = decodedString[indexDivisionforBuffer..];
                        queuedTasks.Add(CreateNewWorker(decodedString, 0, indexDivisionforTasks, cancellationToken));
                        queuedTasks.Add(CreateNewWorker(decodedString, indexDivisionforTasks,
                         indexDivisionforBuffer - indexDivisionforTasks, cancellationToken));
                    }
                    else
                    {
                        queuedTasks.Add(CreateNewWorker(decodedString, 0, decodedString.Length, cancellationToken));
                    }
                buffer = new byte[bufferSize];
                }
                textParser.AddText(addToNextString);
                

                Task.WaitAll(queuedTasks.ToArray());
                return TakeTop10Triplets();
            }
            catch (OperationCanceledException)
            {
                return TakeTop10Triplets();
            }
        }

        private Task CreateNewWorker(string text, int startIndex, int length, CancellationToken token)
        {
            return Task.Run(() =>
            {
                textParser.AddText(text.Substring(startIndex, length));
            }, token);
        }

        private int SearchDivisionIndex(string text, int indexPreDivision)
        {
            int indexDivision = text.LastIndexOf(" ", indexPreDivision);
            return indexDivision;
        }

        private string TakeTop10Triplets()
        {
            return string.Join(", ", textParser.DictionaryOfTripplets.OrderByDescending(tripplet =>
                tripplet.Value).Take(10).Select(item => item.Key));
        }

        public void ZeroingDictionary()
        {
            textParser.DictionaryOfTripplets = new ConcurrentDictionary<string, int>();
        }
        public int TakeCountTriplet(string tripplet)
        {
            return textParser.DictionaryOfTripplets[tripplet];
        }
    }
}
