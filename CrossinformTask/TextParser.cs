using System;
using System.Linq;
using System.Collections.Concurrent;

namespace CrossinformTask
{
    public class TextParser
    {
        private ConcurrentDictionary<string, int> dictionaryOfTripplets = new ConcurrentDictionary<string, int>();
        public ConcurrentDictionary<string, int> DictionaryOfTripplets
        {
            get { return dictionaryOfTripplets; }
            set { dictionaryOfTripplets = value; }
        }

        public void AddText(string text)
        {
            var words = SplitTextOnWords(text);
            AddTripletsToDictionary(words, 3);
        }

        private string[] SplitTextOnWords(string text)
        {
            char[] sep = { ' ', '\r', '\n' };
            return text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        }

        private void AddTripletsToDictionary(string[] words, int lengthTriplets)
        {
            for (int i = 0; i < words.Length; i++)
            {
                for (int j = lengthTriplets; j <= words[i].Length; j++)
                {
                    if (char.IsLetter(words[i][j - lengthTriplets]) &&
                        char.IsLetter(words[i][j - lengthTriplets + 1]) &&
                        char.IsLetter(words[i][j - lengthTriplets + 2]))
                    {
                        var triplet = words[i].Substring(j - lengthTriplets, lengthTriplets);
                        dictionaryOfTripplets.AddOrUpdate(triplet,1, (key, oldValue) => oldValue + 1);
                    }
                }
            }
        }
        public string TakeTop10Triplets()
        {
            return string.Join(", ", dictionaryOfTripplets.OrderByDescending(tripplet => tripplet.Value).Take(10).Select(item => item.Key));
        }
    }
}
