using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MongoDB.Driver;
using Udenad.Core;

namespace Udenad.Import
{
    internal static class Program
    {
        private static IEnumerable<string> Abc => new[]
        {
            "a", "b", "c", "d", "e", "f", "g", "h",
            "i", "j", "k", "l", "m", "n", "o", "p",
            "q", "r", "s", "t", "u", "v", "w", "x",
            "y", "z", "æ", "ø", "å"
        };

        private static IMongoCollection<Card> CardsCollection =>
            new MongoClient()
                .GetDatabase("udenad")
                .GetCollection<Card>("cards");

        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            // WARNING: Both method are provided for demonstration only purpose.
            // Before fetching data from public services, check if copyrights and other terms allow that.
            await FetchWordsAsync();
            await FetchWordDefinitionsAsync();
        }

        private static async Task FetchWordDefinitionsAsync()
        {
            var cursor = await CardsCollection.FindAsync(
                Builders<Card>.Filter.Eq<string>(nameof(Card.Definitions), null) &
                Builders<Card>.Filter.Eq<string>(nameof(Card.Inflection), null) &
                Builders<Card>.Filter.Eq<string>(nameof(Card.WordClass), null));

            while (cursor.MoveNext())
                foreach (var word in cursor.Current
                    .Select(w => w.Word)
                    .AsParallel())
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Word: {word}");

                    var doc = new HtmlWeb()
                        .Load($"http://ordnet.dk/ddo/ordbog?query={word}");

                    // substantiv, fælleskøn
                    var wordclass = WebUtility.HtmlDecode(
                        doc
                            .DocumentNode
                            .SelectNodes(@"//*[@id=""content""]/div[3]/div/div[1]/span[2]")?
                            .First()
                            .InnerText);
                    Console.WriteLine($"\tWord class: {wordclass}");

                    // gaflen eller (uofficielt) -en, gafler, gaflerne
                    var inflection =
                        doc
                            .DocumentNode
                            .SelectNodes(@"//*[@id=""id-boj""]/span[2]")?
                            .Select(n => WebUtility.HtmlDecode(n.InnerText))
                            .First();
                    Console.WriteLine($"\tInflection: {inflection}");

                    // spiseredskab med 2-4 parallelle, let bøjede grene på et skaft
                    var definitions =
                        doc
                            .DocumentNode
                            .SelectNodes(@"//*[@id=""content-betydninger""]//*[@class=""definition""]")?
                            .Select(n => WebUtility.HtmlDecode(n.InnerText))
                            .ToArray();

                    foreach (var definition in definitions ?? Enumerable.Empty<string>())
                        Console.WriteLine($"\tDefinition: {definition}");

                    await UpsertWord(new Card
                    {
                        Word = word,
                        WordClass = wordclass,
                        Inflection = inflection,
                        Definitions = definitions
                    });
                }
        }

        private static async Task UpsertWord(Card card)
        {
            await CardsCollection
                .ReplaceOneAsync(
                    Builders<Card>.Filter.Eq(nameof(Card.Word), card.Word),
                    card,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    });
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
            if (string.IsNullOrEmpty(word)) return;

            await UpsertWord(
                new Card
                {
                    Word = word
                });

            Console.WriteLine(word);
        }
    }
}