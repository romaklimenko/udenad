using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Udenad.Core
{
    public class Card
    {
        private static int[] Fibonacci => new []
        {
            0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181
        };

        [BsonId]
        public string Word { get; set; }

        public string WordClass { get; set; }
        public string Inflection { get; set; }
        public string[] Definitions { get; set; }
        public string Notes { get; set; }

        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        public DateTime? NextDate { get; protected set; }

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