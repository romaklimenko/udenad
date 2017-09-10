using Microsoft.AspNetCore.Mvc;

namespace Udenad.Web
{
    //
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}