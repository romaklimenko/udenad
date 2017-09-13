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
        private IMongoCollection<Card> CardsCollection =>
            new MongoClient()
                .GetDatabase("udenad")
                .GetCollection<Card>("cards");
        
        private IMongoCollection<Count> CountsCollection =>
            new MongoClient()
                .GetDatabase("udenad")
                .GetCollection<Count>("counts");

        public async Task<Card> GetCardAsync(string word)
        {
            return await CardsCollection.Find(c => c.Word == word).SingleOrDefaultAsync();
        }
        
        public async Task<Card> GetNextCardAsync()
        {
            var due = CardsCollection.FindAsync(c => c.NextDate <= DateTime.Today); // 1. due date
            var bad = CardsCollection.FindAsync(c => (int) c.Score < 3);            // 2. bad score
            var unseen = FindRandomOneAsync(c => c.NextDate == null);               // 3. new

            await Task.WhenAll(due, bad, unseen);

            var cards = (await due.Result.ToListAsync())
                .Union(await bad.Result.ToListAsync())
                .Union(unseen.Result == null ?
                    Enumerable.Empty<Card>() : Enumerable.Repeat(unseen.Result, 1))
                .ToArray();

            return cards
                .Skip(new Random().Next(cards.Length))
                .FirstOrDefault();
        }

        private async Task<Card> FindRandomOneAsync(Expression<Func<Card, bool>> filter)
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

        public async Task<Count> GetCountAsync(DateTime date) =>
            await CountsCollection.Find(c => c.Date == date).FirstOrDefaultAsync();

        public async Task<IEnumerable<Count>> GetCountsAsync() =>
            await CountsCollection.Find(c => true).SortBy(c => c.Date).ToListAsync();

        public async Task SaveCardAsync(Card card)
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

        private async Task SaveCountAsync(Count count)
        {
            await CountsCollection
                .ReplaceOneAsync(
                    Builders<Count>.Filter.Eq(nameof(Count.Date), count.Date),
                    count,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    });
        }

        public async Task SaveCountsAsync()
        {
            // WONTFIX: it is slow but it is ok for now
            var all = CountAsync(c => true);
            var bad = CountAsync(c => (int)c.Score < 3);
            var due = CountAsync(c => c.NextDate <= DateTime.Today);
            var mature = CountAsync(c => c.NextDate > DateTime.Today.Date.AddDays(21));
            var unseen = CountAsync(c => c.NextDate == null);
            
            await Task.WhenAll(all, mature, unseen);

            await SaveCountAsync(
                new Count
                {
                    Date = DateTime.Today.Date,
                    All = all.Result,
                    Bad = bad.Result,
                    Due = due.Result,
                    Mature = mature.Result,
                    Unseen = unseen.Result
                });
        }

        private async Task<long> CountAsync(Expression<Func<Card, bool>> expression) =>
            await CardsCollection.CountAsync(expression);
    }
}