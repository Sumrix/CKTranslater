using System;
using System.IO;
using System.Linq;
using NameTranslation;
using NameTranslation.Matching;
using NameTranslation.Storages;
using NameTranslation.Transliteration;
using NameTranslation.Web;

namespace ConsoleTesting
{
    /// <summary>
    ///     Методы для ручного тестирования модуля перевода
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Program.NewMatches();
            //DB.Save();
            //Program.TestTimer();
            //Program.WikiTest();
            //Program.TestNewTranslator();
            Program.TestNotMatchingWikiTranslations();

            //Console.ReadKey();

            DB.Save();
        }

        //private static void RepairSimilaritiesDB()
        //{
        //    EngToRusSimilaritiesDB.Matrix oldItems = null;

        //    if (File.Exists(FileName.EngToRusSimilarities))
        //    {
        //        using (StreamReader file = File.OpenText(FileName.EngToRusSimilarities))
        //        {
        //            JsonSerializer serializer = new JsonSerializer();
        //            oldItems = (EngToRusSimilaritiesDB.Matrix)serializer.Deserialize(file, typeof(EngToRusSimilaritiesDB.Matrix));
        //        }
        //    }

        //    var newDB = new EngToRusSimilaritiesDB();

        //    for (int rusLetter = 0; rusLetter < newDB.RusCount - 1; rusLetter++)
        //    {
        //        for (int engLetter = 0; engLetter < newDB.EngCount - 1; engLetter++)
        //        {
        //            newDB[engLetter, rusLetter] = oldItems.Items[rusLetter, engLetter];
        //        }
        //    }

        //    newDB.Save(FileName.EngToRusSimilarities);
        //}

        private static void NewMatches()
        {
            Language language0 = Language.Load(DB.EngLetters);
            Language language1 = Language.Load(DB.RusLetters);

            (string, string)[] paris =
            {
                //("yffe", "иф"),
                ("richuin", "ришьон")
            };

            foreach ((string w0, string w1) in paris)
            {
                WordMatch match = WordMatch.Create(w0, w1, language0, language1);

                Console.WriteLine($"Words: {w0}, {w1}");
                Console.Write("Matches:");

                foreach (LettersMatch m in match.LetterMatches)
                {
                    Console.Write($" {m.Letters0}-{m.Letters1}");
                }

                Console.WriteLine();

                foreach (LettersMatch lm in match.LetterMatches)
                {
                    Console.WriteLine($" {lm.Letters0}-{lm.Letters1}-{lm.Similarity}");
                }

                float similarity = match.Similarity;
                Console.WriteLine($"\nSimilarity: {similarity}");
                Console.WriteLine();
            }
        }

        private static void TestNewTranslator()
        {
            string[] toTranslateWords = File.ReadAllLines(FileName.ToTranslateWords);

            var translatedWords = Translator.Translate(toTranslateWords);

            File.WriteAllLines(
                @"D:\Desktop\CK2Works\Translations.txt",
                translatedWords.Select(wordInLangs => wordInLangs.ToString())
            );
        }

        private static void TestNotMatchingWikiTranslations()
        {
            Language language1 = Language.Load(DB.EngLetters);
            Language language2 = Language.Load(DB.RusLetters);
            var rusWords = File.ReadAllLines(@"D:\Desktop\CK2Works\Dictionaries\word_rus.txt")
                .ToHashSet();

            File.WriteAllLines(@"D:\Desktop\out.txt",
                DB.Translations
                    .Where(w =>
                        !WordMatch.Create(w.Lang1Word, w.Lang2Word, language1, language2).Success
                        && !rusWords.Contains(w.Lang2Word))
                    .Select(w => w.ToString())
            );
        }

        private static void TestTimer()
        {
            QueueTimer timer = new QueueTimer(1000);
            timer.Tick += (s, e) => Console.WriteLine("1");
            timer.Tick += (s, e) => Console.WriteLine("2");
            timer.Tick += (s, e) => Console.WriteLine("3");
            timer.Tick += (s, e) => Console.WriteLine("4");
            timer.WaitMyTurn();
            Console.WriteLine("5");
        }

        private static void WikiTest()
        {
            Language language0 = Language.Load(DB.EngLetters);
            Language language1 = Language.Load(DB.RusLetters);

            //string[] toTranslateWords = File.ReadAllLines(FileName.ToTranslateWords);
            //var translatedWords = Wiki.Translate(toTranslateWords, language0, language1)
            //    .OrderBy(x => x.Lang1Word);
            //File.WriteAllLines(@"D:\Desktop\out.txt", translatedWords.Select(x => x.ToString()).ToArray());

            var v = Wiki.Translate(new[] { "aceto" }, language0, language1)
                .ToList();

            //File.WriteAllLines(
            //    @"D:\Desktop\Wiki.GetSimilar.txt",
            //    Wiki.TranslateExact(Wiki.Search("Abi'l-Hadid"))
            //        .Select(x => x.ToString())
            //        .ToArray()
            //);

            // У prefix search результаты лучше
            //var s1 = WikiApi.Search("abah");
            //var s2 = WikiApi.PrefixSearch("abah");
        }

        //private static void PrepareToManualTranslate()
        //{
        //    string wordsFileName = @"D:\Desktop\CK2Works\NotTransleated.txt";
        //    string englishWordsFileName = @"D:\Desktop\CK2Works\Dictionaries\words.txt";

        //    HashSet<string> engWords = File.ReadAllLines(englishWordsFileName)
        //           .Select(w => w.ToLower())
        //           .ToHashSet();
        //    char[] forbiddenChars = "()[]§".ToArray();
        //    HashSet<char> allowedChars = @" '-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzÀÁÂÄÅÆÇÉÍÎÑÒÓÖØÚÜÞßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþœŠšŽž"
        //        .ToHashSet();

        //    List<string> words = File.ReadAllLines(wordsFileName)
        //        .Where(line => line.Any(char.IsLetter) &&
        //                    !line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
        //                        .Any(word => engWords.Contains(word.ToLower())) && // Не является предложением
        //                    !forbiddenChars.Any(c => line.Contains(c))  //Не содержит запрещённые символы
        //        )
        //        .Select(line => CharEncoding.Decode(line)
        //                            .Replace('‘', '\'')
        //                            .Replace('`', '\'')
        //                            .Replace('’', '\'')
        //                            .Replace('´', '\'')
        //                            .Replace('‚', ',')
        //                            .Replace('¸', ',')
        //                            .Replace('—', '-')
        //                            .Replace('–', '-')
        //                            .Replace('“', '"')
        //                            .Replace('”', '"')
        //                            .Replace("…", "...")
        //        )
        //        .Where(line => line.All(c => allowedChars.Contains(c)))
        //        .SelectMany(line => line.ToLower().Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries))
        //        .Distinct()
        //        .OrderBy(x => x)
        //        .ToList();

        //    File.Delete(FileName.ToTranslateWords);
        //    File.WriteAllLines(FileName.ToTranslateWords, words);
        //}
    }
}