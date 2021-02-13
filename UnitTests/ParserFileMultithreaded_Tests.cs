using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace CrossinformTask
{
    [TestFixture]
    class ParserFileMultithreaded_Tests
    {
        private ParserFileMultithreaded readasync = new ParserFileMultithreaded();
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();

        [Test]
        [Order(01)]
        public void UsualTestBigFile()
        {
            readasync.ZeroingDictionary();
            var pathTxtFile = @"CrossinformTask/FileForTests/HarryPotterBigALotOfText.txt";

            var expected = "the, ing, and, rry, arr, Har, was, her, his, ere";

            var actual = readasync.ReadAllFileAsync(pathTxtFile, 1024, new CancellationToken());
            Assert.AreEqual(expected, actual.Result);
        }

        [Test]
        [Order(02)]
        public void OnlyOneTrippleInOneWord()
        {
            readasync.ZeroingDictionary();
            var pathTxtFile = @"CrossinformTask/FileForTests/OnlyOneTrippleInOneWord.txt";

            var expected = "the";

            var actual = readasync.ReadAllFileAsync(pathTxtFile, 1024, new CancellationToken());
            Assert.AreEqual(expected, actual.Result);
        }

        [Test]
        [Order(03)]
        public void EmptyFile()
        {
            readasync.ZeroingDictionary();
            var pathTxtFile = @"CrossinformTask/FileForTests/EmptyFile.txt";

            var expected = "";

            var actual = readasync.ReadAllFileAsync(pathTxtFile, 1024, new CancellationToken());
            Assert.AreEqual(expected, actual.Result);
        }

        [Test]
        [Order(04)]
        public void CountTripplet()
        {
            readasync.ZeroingDictionary();
            var pathTxtFile = @"CrossinformTask/FileForTests/HarryPotterText.txt";

            var text = File.ReadAllText(pathTxtFile);

            string triplet = "the";
            var expected = new Regex($"{triplet}").Matches(text).Count;

            var actual = readasync.ReadAllFileAsync(pathTxtFile, 1024, new CancellationToken());
            var result = actual.Result;
            Assert.AreEqual(expected, readasync.TakeCountTriplet(triplet));
        }
    }
}
