using Microsoft.AspNetCore.Mvc;

namespace AkkaExchange.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}