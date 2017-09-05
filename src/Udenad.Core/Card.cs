using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Udenad.Core
{
    public class Card
    {
        public const double SmallestEasiness = 1.3;
        public const int BiggestLastInterval = 365 * 3;

        private double _easiness = SmallestEasiness;
        private int _lastInterval;

        [BsonId]
        public string Word { get; set; }

        public string WordClass { get; set; }
        public string Inflection { get; set; }
        public string[] Definitions { get; set; }

        public DateTime? NextDate { get; set; }

        public double Easiness
        {
            get => _easiness;
            set => _easiness = Math.Max(SmallestEasiness, value);
        }

        public int LastInterval
        {
            get => _lastInterval;
            set => _lastInterval = Math.Min(BiggestLastInterval, value);
        }

        public int Repetitions { get; set; }

        public Score Score { get; set; }
    }
}