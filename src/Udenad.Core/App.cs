using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Udenad.Core
{
    public class App
    {
        private static IMongoClient MongoClient =>
            new MongoClient();

        private static IMongoDatabase MongoDatabase =>
            MongoClient.GetDatabase("udenad");

        private static IMongoCollection<Card> CardsCollection =>
            MongoDatabase.GetCollection<Card>("cards");

        private static IMongoCollection<Count> CountsCollection =>
            MongoDatabase.GetCollection<Count>("counts");

        public static async Task<Card> GetCardAsync(string word) =>
            await CardsCollection.Find(c => c.Word == word)
                .SingleOrDefaultAsync();

        public static async Task<Card> GetNextCardAsync()
        {
            var due = CardsCollection.FindAsync(c => c.NextDate <= DateTime.Today); // all due date
            var unseen = FindRandomOneAsync(c => c.NextDate == null);               // and one new

            await Task.WhenAll(due, unseen);

            var cards = (await due.Result.ToListAsync())
                .Union(unseen.Result == null ? Enumerable.Empty<Card>() : Enumerable.Repeat(unseen.Result, 1))
                .ToArray();

            var card = cards
                .Skip(new Random().Next(cards.Length))
                .FirstOrDefault();

            return card;
        }

        private static async Task<Card> FindRandomOneAsync(Expression<Func<Card, bool>> filter)
        {
            var count = await CardsCollection
                .Find(filter)
                .CountAsync();

            if (count == 0) return null;

            return await CardsCollection
                .Find(filter)
                .Skip(new Random()
                    .Next(Convert.ToInt32(Math.Min(int.MaxValue, count))))
                .FirstAsync();
        }

        public static async Task<Count> GetCountAsync(DateTime date) =>
            await CountsCollection.Find(c => c.Date == date).FirstOrDefaultAsync();

        public static async Task<IEnumerable<Count>> GetCountsAsync() =>
            await CountsCollection.Find(c => true).SortBy(c => c.Date).ToListAsync();

        public static async Task<IEnumerable<(DateTime, double, double, double)>> GetForecastAsync()
        {
            var young = (await CardsCollection.Aggregate()
                .Match(c => c.NextDate != null && c.Repetitions < 6)
                .Group(new BsonDocument
                {
                    { "_id", "$NextDate" },
                    { "young", new BsonDocument { { "$sum", 1 } } }
                })
                .Sort(new BsonDocument
                {
                    { "_id", 1 }
                })
                .ToListAsync())
                .Select(
                    r => (Date: r["_id"].ToUniversalTime(), Young: r["young"].ToDouble()))
                .ToDictionary(c => c.Date);

            var almostMature = (await CardsCollection.Aggregate()
                .Match(c => c.NextDate != null && c.Repetitions == 6)
                .Group(new BsonDocument
                {
                    { "_id", "$NextDate" },
                    { "almost_mature", new BsonDocument { { "$sum", 1 } } }
                })
                .Sort(new BsonDocument
                {
                    { "_id", 1 }
                })
                .ToListAsync())
                .Select(
                    r => (Date: r["_id"].ToUniversalTime(), AlmostMature: r["almost_mature"].ToDouble()))
                .ToDictionary(c => c.Date);

            var mature = (await CardsCollection.Aggregate()
                .Match(c => c.NextDate != null && c.Repetitions > 6)
                .Group(new BsonDocument
                {
                    { "_id", "$NextDate" },
                    { "mature", new BsonDocument { { "$sum", 1 } } }
                })
                .Sort(new BsonDocument
                {
                    { "_id", 1 }
                })
                .ToListAsync())
                .Select(
                    r => (Date: r["_id"].ToUniversalTime(), Mature: r["mature"].ToDouble()))
                .ToDictionary(c => c.Date);

            var dates = young.Union(almostMature).Union(mature)
                .Select(d => d.Key)
                .OrderBy(d => d)
                .Distinct();

            var result = new List<(DateTime, double, double, double)>();

            foreach(var date in dates)
            {
                young.TryGetValue(date, out (DateTime, double) _young);
                almostMature.TryGetValue(date, out (DateTime, double) _almostMature);
                mature.TryGetValue(date, out (DateTime, double) _mature);

                result.Add(
                    (
                        date,
                        _young.Item2,
                        _almostMature.Item2,
                        _mature.Item2
                    ));
            }

            return result;
        }

        public static async Task<IEnumerable<(int?, double)>> GetRepetitionsAsync()
        {
            var result = await CardsCollection.Aggregate()
                .Group(new BsonDocument
                {
                    { "_id", "$Repetitions" },
                    { "count", new BsonDocument { { "$sum", 1 } } }
                })
                .Sort(new BsonDocument
                {
                    { "_id", 1 }
                })
                .ToListAsync();

            return result
                .Select(
                    r => (Repetitions: r["_id"] == BsonNull.Value ? null : (int?)r["_id"].ToInt32(), Count: r["count"].ToDouble()));
        }

        public static async Task SaveCardAsync(Card card)
        {
            await CardsCollection
                .ReplaceOneAsync(
                    Builders<Card>.Filter.Eq(nameof(Card.Word), card.Word),
                    card,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    });

            await SaveCountsAsync();
        }

        private static async Task SaveCountAsync(Count count) =>
            await CountsCollection
                .ReplaceOneAsync(
                    Builders<Count>.Filter.Eq(nameof(Count.Date), count.Date),
                    count,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    });

        public static async Task SaveCountsAsync()
        {
            // WONTFIX: it is slow but it is ok for now
            var all = CountAsync(c => true);
            var due = CountAsync(c => c.NextDate <= DateTime.Today);
            var mature = CountAsync(c => c.Repetitions > 6);
            var seen = CountAsync(c => c.NextDate != null);

            await Task.WhenAll(all, mature, seen);

            await SaveCountAsync(
                new Count
                {
                    Date = DateTime.Today.Date,
                    All = all.Result,
                    Due = due.Result,
                    Mature = mature.Result,
                    Seen = seen.Result
                });
        }

        private static async Task<long> CountAsync(Expression<Func<Card, bool>> expression) =>
            await CardsCollection.CountAsync(expression);
    }
}