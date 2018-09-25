using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Udenad.Core;

namespace Udenad.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            App.SaveCountsAsync().GetAwaiter().GetResult();
        }

        // GET /
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            Thread.Sleep(250);

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
        public async Task<JsonResult> Counts() => Json(await App.GetCountsAsync());

        [HttpGet("forecast")]
        public async Task<JsonResult> Forecast() =>
            Json((await App.GetForecastAsync())
                .Select(f => new
                {
                    Date = f.Item1,
                    Count = f.Item2,
                    Average = f.Item3
                }));

        [HttpGet("repetitions")]
        public async Task<JsonResult> Repetitions() =>
            Json((await App.GetRepetitionsAsync())
                .Select(f => new
                {
                    Repetitions = f.Item1,
                    Count = f.Item2
                }));
    }
}