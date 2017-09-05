using System;

namespace Udenad.Core
{
    /// <summary>
    ///     SuperMemo2 implementation https://www.supermemo.com/english/ol/sm2.htm
    /// </summary>
    public static class SuperMemo2Extension
    {
        public static void Review(this Card card, Score score)
        {
            card.Score = score;

            if ((int) score < 3)
                card.Repetitions = 1;
            else
                card.Repetitions++;

            switch (card.Repetitions)
            {
                case 1:
                    // I(1) := 1
                    card.LastInterval = 1;
                    break;
                case 2:
                    // I(2) := 6
                    card.LastInterval = 6;
                    break;
                default:
                    // for n > 2 I(n) := I(n-1) * EF 
                    card.LastInterval = (int) Math.Ceiling(card.LastInterval * card.Easiness);
                    break;
            }

            // EF' := EF - 0.8 + 0.28 * q - 0.02 * q * q
            card.Easiness = Math.Max(
                Card.SmallestEasiness,
                card.Easiness - 0.8 + 0.28 * (int) score - 0.02 * (int) score * (int) score);

            card.NextDate = DateTime.UtcNow.Date.AddDays(card.LastInterval);
        }
    }
}