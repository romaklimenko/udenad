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
        
        public async Task<Card> GetNextCard()
        {
            var result = await FindRandomOneAsync(c => c.NextDate >= DateTime.Today) // 1. due date
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

        public async Task SaveCardAsync(Card card)
        {
            //
        }
    }
}