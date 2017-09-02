using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Udenad.Import
{
    class Program
    {
        private static string[] Abc => new[]
        {
            "a", "b", "c", "d", "e", "f", "g", "h",
            "i", "j", "k", "l", "m", "n", "o", "p",
            "q", "r", "s", "t", "u", "v", "w", "x",
            "y", "z", "æ", "ø", "å"
        };

        private static void Main() =>
            MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            await FetchWordsAsync();
//            await FetchWordDefinitionsAsync();
        }

        private static async Task FetchWordDefinitionsAsync()
        {
            var word = "gaffel";
            var url = $"http://ordnet.dk/ddo/ordbog?query={word}";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            // substantiv, fælleskøn
            var wordclass = WebUtility.HtmlDecode(
                doc
                    .DocumentNode
                    .SelectNodes(@"//*[@id=""content""]/div[3]/div/div[1]/span[2]")
                    .First()
                    .InnerText);
            Console.WriteLine(wordclass);

            // gaflen eller (uofficielt) -en, gafler, gaflerne
            var inflection = WebUtility.HtmlDecode(
                doc
                    .DocumentNode
                    .SelectNodes(@"//*[@id=""id-boj""]/span[2]")
                    .First()
                    .InnerText);
            Console.WriteLine(inflection);

            // spiseredskab med 2-4 parallelle, let bøjede grene på et skaft
            var definitions =
                doc
                    .DocumentNode
                    .SelectNodes(@"//*[@id=""content-betydninger""]//*[@class=""definition""]")
                    .Select(n => WebUtility.HtmlDecode(n.InnerText));

            foreach (var definition in definitions)
            {
                Console.WriteLine(definition);
            }
        }

        private static async Task FetchWordsAsync(string prefix = "")
        {
            if (prefix.Length < 1)
            {
                foreach (var letter in Abc)
                    await FetchWordsAsync($"{prefix}{letter}");
                return;
            }

            foreach (var letter in Abc)
            {
                var text = $"{prefix}{letter}";

                var httpResponseMessage = await new HttpClient()
                    .GetAsync($"http://ordnet.dk/ws/ddo/livesearch?text={text}&size=50");

                var content = await httpResponseMessage
                    .Content
                    .ReadAsStringAsync();

                var words = content
                    .Replace("<html><body><ul>", string.Empty)
                    .Replace("</ul></body></html>", string.Empty)
                    .Replace("<li>", string.Empty)
                    .Split("</li>")
                    .Where(w => !string.IsNullOrEmpty(w))
                    .ToArray();

                foreach (var word in words)
                    await SaveWordAsync(word);

                if (words.Length == 50)
                    await FetchWordsAsync(text);
            }
        }

        private static async Task SaveWordAsync(string word)
        {
            if(string.IsNullOrEmpty(word)) return;

            await new MongoClient()
                .GetDatabase("udenad")
                .GetCollection<WordDocument>("words")
                .ReplaceOneAsync(
                    Builders<WordDocument>.Filter.Eq(nameof(WordDocument.Word), word),
                    new WordDocument
                    {
                        Word = word
                    },
                    new UpdateOptions
                    {
                        IsUpsert = true
                    });

            Console.WriteLine(word);
        }
    }

    public class WordDocument
    {
        [BsonId]
        public string Word { get; set; }

        public string WordClass { get; set; }

        public string Inflection { get; set; }

        public string[] Definitions { get; set; }
    }
}