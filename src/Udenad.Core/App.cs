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

            var cards = due.Result.ToList()
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

        public static async Task<IEnumerable<(DateTime, double, double)>> GetForecastAsync()
        {
            var result = await CardsCollection.Aggregate()
                .Match(c => c.NextDate != null)
                .Group(new BsonDocument
                {
                    { "_id", "$NextDate" },
                    { "count", new BsonDocument { { "$sum", 1 } } },
                    { "average", new BsonDocument { { "$avg", "$Repetitions" } } }
                })
                .Sort(new BsonDocument
                {
                    { "_id", 1 }
                })
                .ToListAsync();

            return result
                .Select(r =>
                    (
                        Date: r["_id"].ToUniversalTime(),
                        Count: r["count"].ToDouble(),
                        Average: r["average"].ToDouble()
                    )
                );
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
            if (card.NextDate != null && card.Repetitions != 0)
            {
                if (card.Repetitions > 6)
                {
                    // we allow more matutes than youngs in a single day
                    while (await CardsCollection
                        .CountAsync(c => c.NextDate == card.NextDate && c.Repetitions > 6) > 99)
                    {
                        card.NextDate = card.NextDate?.AddDays(1);
                    }
                }
                else
                {
                    while (await CardsCollection
                        .CountAsync(c => c.NextDate == card.NextDate && c.Repetitions < 7) > 50)
                    {
                        card.NextDate = card.NextDate?.AddDays(1);
                    }
                }
            }

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

            await Task.WhenAll(all, due, mature, seen);

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