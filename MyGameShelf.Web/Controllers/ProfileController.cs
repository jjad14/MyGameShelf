using Microsoft.AspNetCore.Mvc;

namespace MyGameShelf.Web.Controllers;

[Route("profile")]
public class ProfileController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
