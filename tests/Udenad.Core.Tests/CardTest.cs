using System;
using Xunit;

namespace Udenad.Core.Tests
{
    public class CardTest
    {
        [Theory]
        [InlineData(Score.S5, 1.4, 1)]
        [InlineData(Score.S4, Card.SmallestEasiness, 1)]
        [InlineData(Score.S3, Card.SmallestEasiness, 1)]
        [InlineData(Score.S2, Card.SmallestEasiness, 1)]
        [InlineData(Score.S1, Card.SmallestEasiness, 1)]
        [InlineData(Score.S0, Card.SmallestEasiness, 1)]
        public void Review001(Score score, double easiness, int lastInterval)
        {
            var card = new Card();
            card.Review(score);

            Assert.Equal(easiness, card.Easiness, 5);
            Assert.Equal(lastInterval, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(lastInterval), card.NextDate);
            Assert.Equal(1, card.Repetitions);
        }

        [Theory]
        [InlineData(Score.S5, 1.5, 6)]
        [InlineData(Score.S4, Card.SmallestEasiness, 6)]
        [InlineData(Score.S3, Card.SmallestEasiness, 6)]
        [InlineData(Score.S2, Card.SmallestEasiness, 1)]
        [InlineData(Score.S1, Card.SmallestEasiness, 1)]
        [InlineData(Score.S0, Card.SmallestEasiness, 1)]
        public void Review002(Score score, double easiness, int lastInterval)
        {
            var card = new Card();

            for (var i = 0; i < 2; i++)
            {
                card.Review(score);
                Assert.Equal((int) score < 3 ? 1 : i + 1, card.Repetitions);
            }

            Assert.Equal(easiness, card.Easiness);
            Assert.Equal(lastInterval, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(lastInterval), card.NextDate);
        }

        [Theory]
        [InlineData(Score.S5, 1.6, 9)]
        [InlineData(Score.S4, Card.SmallestEasiness, 8)]
        [InlineData(Score.S3, Card.SmallestEasiness, 8)]
        [InlineData(Score.S2, Card.SmallestEasiness, 1)]
        [InlineData(Score.S1, Card.SmallestEasiness, 1)]
        [InlineData(Score.S0, Card.SmallestEasiness, 1)]
        public void Review003(Score score, double easiness, int lastInterval)
        {
            var card = new Card();

            for (var i = 0; i < 3; i++)
            {
                card.Review(score);
                Assert.Equal((int) score < 3 ? 1 : i + 1, card.Repetitions);
            }

            Assert.Equal(easiness, card.Easiness, 3);
            Assert.Equal(lastInterval, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(lastInterval), card.NextDate);
        }

        [Theory]
        [InlineData(Score.S5, 1.7, 15)]
        [InlineData(Score.S4, Card.SmallestEasiness, 11)]
        [InlineData(Score.S3, Card.SmallestEasiness, 11)]
        [InlineData(Score.S2, Card.SmallestEasiness, 1)]
        [InlineData(Score.S1, Card.SmallestEasiness, 1)]
        [InlineData(Score.S0, Card.SmallestEasiness, 1)]
        public void Review004(Score score, double easiness, int lastInterval)
        {
            var card = new Card();

            for (var i = 0; i < 4; i++)
            {
                card.Review(score);
                Assert.Equal((int) score < 3 ? 1 : i + 1, card.Repetitions);
            }

            Assert.Equal(easiness, card.Easiness, 3);
            Assert.Equal(lastInterval, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(lastInterval), card.NextDate);
        }

        [Theory]
        [InlineData(Score.S5, 2.3, 839)]
        [InlineData(Score.S4, Card.SmallestEasiness, 59)]
        [InlineData(Score.S3, Card.SmallestEasiness, 59)]
        [InlineData(Score.S2, Card.SmallestEasiness, 1)]
        [InlineData(Score.S1, Card.SmallestEasiness, 1)]
        [InlineData(Score.S0, Card.SmallestEasiness, 1)]
        public void Review010(Score score, double easiness, int lastInterval)
        {
            var card = new Card();

            for (var i = 0; i < 10; i++)
            {
                card.Review(score);
                Assert.Equal((int) score < 3 ? 1 : i + 1, card.Repetitions);
            }

            Assert.Equal(easiness, card.Easiness, 3);
            Assert.Equal(lastInterval, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(lastInterval), card.NextDate);
        }

        [Theory]
        [InlineData(Score.S5, 11.3, Card.BiggestLastInterval)]
        [InlineData(Score.S4, Card.SmallestEasiness, Card.BiggestLastInterval)]
        [InlineData(Score.S3, Card.SmallestEasiness, Card.BiggestLastInterval)]
        [InlineData(Score.S2, Card.SmallestEasiness, 1)]
        [InlineData(Score.S1, Card.SmallestEasiness, 1)]
        [InlineData(Score.S0, Card.SmallestEasiness, 1)]
        public void Review100(Score score, double easiness, int lastInterval)
        {
            var card = new Card();

            for (var i = 0; i < 100; i++)
            {
                card.Review(score);
                Assert.Equal((int) score < 3 ? 1 : i + 1, card.Repetitions);
            }

            Assert.Equal(easiness, card.Easiness, 3);
            Assert.Equal(lastInterval, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(lastInterval), card.NextDate);
        }

        [Fact]
        public void ComplexReview()
        {
            var card = new Card();

            card.Review(Score.S0);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(1, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(1), card.NextDate);

            card.Review(Score.S1);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(1, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(1), card.NextDate);

            card.Review(Score.S2);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(1, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(1), card.NextDate);

            card.Review(Score.S3);

            Assert.Equal(2, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(6, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(6), card.NextDate);

            card.Review(Score.S4);

            Assert.Equal(3, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(8, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(8), card.NextDate);

            card.Review(Score.S5);

            Assert.Equal(4, card.Repetitions);
            Assert.Equal(1.4, card.Easiness, 3);
            Assert.Equal(11, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(11), card.NextDate);

            card.Review(Score.S4);

            Assert.Equal(5, card.Repetitions);
            Assert.Equal(1.4, card.Easiness, 3);
            Assert.Equal(16, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(16), card.NextDate);

            card.Review(Score.S3);

            Assert.Equal(6, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(23, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(23), card.NextDate);

            card.Review(Score.S2);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(1, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(1), card.NextDate);

            card.Review(Score.S1);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(1, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(1), card.NextDate);

            card.Review(Score.S0);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(Card.SmallestEasiness, card.Easiness, 3);
            Assert.Equal(1, card.LastInterval);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(1), card.NextDate);
        }
    }
}