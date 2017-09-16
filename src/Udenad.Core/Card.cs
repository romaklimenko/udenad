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
        public string Notes { get; set; }

        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        public DateTime? NextDate { get; protected set; }

        public double Easiness
        {
            get => _easiness;
            private set => _easiness = Math.Max(SmallestEasiness, value);
        }

        public int LastInterval
        {
            get => _lastInterval;
            private set => _lastInterval = Math.Min(BiggestLastInterval, value);
        }

        public int Repetitions { get; private set; }

        public Score Score { get; private set; }
        
        public void Review(Score score)
        { 
            if (NextDate != null && DateTime.Today < NextDate)
                return;

            Score = score;

            if ((int) score < 3)
            {
                Repetitions = 0;
            }
            else
            {
                Repetitions++;
            }
            
            switch (Repetitions)
            {
                case 0:
                    LastInterval = 0;
                    break;
                case 1:
                    // I(1) := 1
                    LastInterval = 1;
                    break;
                case 2:
                    // I(2) := 6
                    LastInterval = 6;
                    break;
                default:
                    // for n > 2 I(n) := I(n-1) * EF 
                    LastInterval = (int) Math.Ceiling(LastInterval * Easiness);
                    break;
            }

            // EF' := EF - 0.8 + 0.28 * q - 0.02 * q * q
            Easiness = Math.Max(
                SmallestEasiness,
                Easiness - 0.8 + 0.28 * (int) score - 0.02 * (int) score * (int) score);

            NextDate = DateTime.Now.Date.AddDays(LastInterval);
        }
    }
}