using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Udenad.Core;

namespace Udenad.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            App.SaveCountsAsync().GetAwaiter().GetResult();
        }

        // GET /
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var card = App.GetNextCardAsync();
            var count = App.GetCountAsync(DateTime.Today);

            await Task.WhenAll(card, count);

            return View((Card: card.Result, Count: count.Result));
        }

        [HttpPost("notes")]
        public async Task<IActionResult> Notes(string word, string notes)
        {
            var card = await App.GetCardAsync(word);
            if (card == null)
                return NotFound();

            card.Notes = notes;

            await App.SaveCardAsync(card);

            return Ok(card);
        }
        
        [HttpPost("score")]
        public async Task<IActionResult> Score(string word, bool score)
        {
            var card = await App.GetCardAsync(word);
            if (card == null)
                return NotFound();

            card.Review(score);

            await App.SaveCardAsync(card);
            
            return Ok();
        }

        [HttpGet("charts")]
        public IActionResult Charts() => View();
        
        [HttpGet("counts")]
        public async Task<JsonResult> Counts() =>
            Json(await App.GetCountsAsync());

        [HttpGet("forecast")]
        public async Task<JsonResult> Forecast() =>
            Json((await App.GetForecastAsync())
                .Select(f => new
                {
                    Date = f.Item1,
                    Count = f.Item2
                }));


    }
}