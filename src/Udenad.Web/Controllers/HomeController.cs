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
        [ResponseCache(Duration = 0)]
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

            return Ok();
        }

        [HttpGet("charts")]
        public IActionResult Charts() => View();
        
        [HttpGet("counts")]
        public async Task<JsonResult> Counts() =>
            Json(await _app.GetCountsAsync());
    }
}