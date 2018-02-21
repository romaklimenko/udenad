using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Udenad.Core
{
    public class Card
    {
        [BsonId] public string Word { get; set; }

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

            Repetitions = !score ? 0 : Repetitions + 1;

            NextDate = DateTime.Now.Date.AddDays(Fibonacci(Repetitions));
        }

        private static int Fibonacci(int step)
        {
            //  step: 0, 1, 2, 3, 4, 5,  6,  7,  8,  9, 10, ...
            // value: 0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, ...
            
            if (step <= 3) return Math.Max(0, step); // 0, 1, 2, 3 ...

            int first = 2, second = 3, result = 0;

            for (var i = 4; i <= step; i++) // from step 4
            {
                result = first + second;
                first = second;
                second = result;
            }

            return result;
        }
    }
}