using System;
using Xunit;

namespace Udenad.Core.Tests
{
    public class TestCard : Card
    {
        public void Today() => NextDate = DateTime.Today;
    }

    public class CardTest
    {
        [Fact]
        public void Review()
        {
            // 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181

            var card = new TestCard();
            card.Today();
            card.Review(true);

            Assert.Equal(1, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(1), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(2, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(2), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(3, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(3), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(4, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(5), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(5, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(8), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(6, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(13), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(7, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(21), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(8, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(34), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(9, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(55), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(10, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(89), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(11, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(144), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(12, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(233), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(13, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(377), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(14, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(610), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(15, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(987), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(16, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(1597), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(17, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(2584), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(18, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(4181), card.NextDate);

            card.Today();
            card.Review(true);

            Assert.Equal(19, card.Repetitions);
            Assert.Equal(DateTime.Today.AddDays(6765), card.NextDate);

            card.Today();
            card.Review(false);

            Assert.Equal(0, card.Repetitions);
            Assert.Equal(DateTime.Today, card.NextDate);
        }
    }
}