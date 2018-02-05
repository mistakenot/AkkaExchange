using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AkkaExchange.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home.Index called");
            return View();
        }
    }
}