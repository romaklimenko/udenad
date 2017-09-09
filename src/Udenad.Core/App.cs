using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        
        public async Task<Card> GetNextCard()
        {
            var result = await FindRandomOneAsync(c => c.NextDate <= DateTime.Today) // 1. due date
                         ??
                         await FindRandomOneAsync(c => (int) c.Score < 3); // 2. bad score

            return result ?? await FindRandomOneAsync(c => c.NextDate == null); // 3. new
        }

        private async Task<Card> FindRandomOneAsync(Expression<Func<Card, bool>> filter)
        {
            var count = await CardsCollection
                .Find(filter)
                .CountAsync();
            return await CardsCollection
                .Find(filter)
                .Skip(new Random()
                    .Next(Convert.ToInt32(Math.Min(int.MaxValue, count))))
                .FirstOrDefaultAsync();
        }

        public async Task<Count> GetCountAsync(DateTime date) =>
            await CountsCollection.Find(c => c.Date == date).FirstOrDefaultAsync();

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

        private async Task SaveCountsAsync()
        {
            // WONTFIX: it is slow but it is ok for now
            var all = CountAsync(c => true);
            var mature = CountAsync(c => c.NextDate > DateTime.Today.Date.AddDays(21));
            var unseen = CountAsync(c => c.NextDate == null);
            
            await Task.WhenAll(all, mature, unseen);

            await SaveCountAsync(
                new Count
                {
                    Date = DateTime.Today.Date,
                    All = all.Result,
                    Mature = mature.Result,
                    Unseen = unseen.Result
                });
        }

        private async Task<long> CountAsync(Expression<Func<Card, bool>> expression) =>
            await CardsCollection.CountAsync(expression);
    }
}