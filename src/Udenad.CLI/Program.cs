using System;
using System.Threading.Tasks;
using Udenad.Core;

namespace Udenad.CLI
{
    static class Program
    {
        private static App _app = new App();
        private static Card _card;

        private static void Main() =>
            MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync() =>
            await FrontSideState();

        #region States

        private static async Task FrontSideState()
        {
            _card = await _app.GetNextCard();
            Console.WriteLine("🇩🇰");
            Console.WriteLine("Ord:");
            Console.WriteLine($" {_card.Word}");
            ReadEnter();
            await BackSideState();
        }

        private static async Task BackSideState()
        {
            Console.WriteLine("Ordklasse:");
            Console.WriteLine($" {_card.WordClass}");
            Console.WriteLine("Bøjning:");
            Console.WriteLine($" {_card.Inflection}");
            Console.WriteLine("Betydninger:");
            foreach (var definition in _card.Definitions ?? new string[]{})
            {
                Console.WriteLine($" • {definition}");
            }
            Console.WriteLine("Noter:");
            Console.WriteLine($" {_card.Notes}");
            Console.WriteLine();
            Console.Write("Now type your score from 0 to 5: ");
            await ScoreState(ReadScore());
        }

        private static async Task ScoreState(Score score)
        {
            _card.Review(score);
            await _app.SaveCardAsync(_card);
            Console.WriteLine();
            Console.WriteLine("--------");
            Console.WriteLine();
            await FrontSideState();
        }

        private static Score ReadScore()
        {
            if (!int.TryParse(Console.ReadLine(), out var score) || score <= 0 || score >= 6)
            {
                return ReadScore();
            }
            Console.WriteLine($"Your score is: {score}");
            return (Score) score;
        }

        private static void ReadEnter()
        {
            if (Console.ReadKey(true).Key != ConsoleKey.Enter) ReadEnter();
        }
        
        #endregion
    }
}