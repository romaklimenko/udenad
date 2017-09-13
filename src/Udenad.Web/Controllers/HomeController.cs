using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Udenad.Core;

namespace Udenad.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly App _app;

        public HomeController(App app)
        {
            _app = app;
            _app.SaveCountsAsync().GetAwaiter().GetResult();
        }


        // GET /
        public async Task<IActionResult> Index()
        {
            var card = await _app.GetNextCardAsync();
            var count = await _app.GetCountAsync(DateTime.Today);

            return View((Card: card, Count: count));
        }

        [HttpPost("score")]
        public async Task<IActionResult> Score(string word, Score score)
        {
            var card = await _app.GetCardAsync(word);
            if (card == null)
                return NotFound();

            card.Review(score);

            await _app.SaveCardAsync(card);
            await _app.SaveCountsAsync();

            return Ok();
        }

        [HttpGet("charts")]
        public async Task<IActionResult> Charts() =>
            View(await _app.GetCountsAsync());
    }
}