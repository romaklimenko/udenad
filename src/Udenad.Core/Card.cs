using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Udenad.Core
{
    public class Card
    {
        private static int[] Fibonacci => new []
        {
            0,      // impossible
            1,      // 1st, 0 days
            2,      // 2nd, 1 day
            3,      // 3rd, 3 days
            5,      // 4th, 6 days
            8,      // 5th, 11 days
            13,     // 6th, 19 days
            21,     // 7th, 32 days => the card becomes mature
            34,     // 8th, 53 days
            55,     // 9th, 108 days
            89,     // 10th, 197 days
            144,    // 11th, 286 days
            233,    // 12th
            377,    // 13th
            610,    // 14th
            987,    // 15th
            1597,   // 16th
            2584,   // 17th
            4181    // 18th
        };

        [BsonId]
        public string Word { get; set; }

        public string WordClass { get; set; }
        public string Inflection { get; set; }
        public string[] Definitions { get; set; }
        public string Notes { get; set; }

        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        public DateTime? NextDate { get; set; }

        [BsonIgnoreIfNull]
        public string Audio { get; set; }

        public int Repetitions { get; private set; }

        public void Review(bool score)
        {
            if (NextDate != null && DateTime.Today < NextDate)
                return;

            Repetitions = !score ? 0 : Math.Min(Fibonacci.Length - 1, Repetitions + 1);

            NextDate = DateTime.Now.Date.AddDays(Fibonacci[Repetitions]);
        }
    }
}