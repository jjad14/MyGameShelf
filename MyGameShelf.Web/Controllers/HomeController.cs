using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Web.Models;

namespace MyGameShelf.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<BaseController> _logger;

        public HomeController(ILogger<BaseController> logger) : base(logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
