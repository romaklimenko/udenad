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
            Console.WriteLine("Score:");
            var score = ReadScore(Score.S3);
            Console.WriteLine($"  {score.GetDescription()}");
            Console.WriteLine();
            await ScoreState(score);
        }

        private static async Task ScoreState(Score score)
        {            
            _card.Review(score);
            await _app.SaveCardAsync(_card);
            await FrontSideState();
        }

        private static Score ReadScore(Score score)
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
            Console.Write($"↓ {score.GetDescription()} ↑");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    return (byte) score == 5 ?
                        ReadScore(score) : ReadScore((Score) ((byte) score + 1));
                case ConsoleKey.DownArrow:
                    return (byte) score == 0 ?
                        ReadScore(score) : ReadScore((Score) ((byte) score - 1));
                case ConsoleKey.Enter:
                    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                    return score;
                default:
                    return ReadScore(score);
            }
        }

        private static void ReadEnter()
        {
            if (Console.ReadKey(true).Key != ConsoleKey.Enter) ReadEnter();
        }
        
        #endregion
    }
}