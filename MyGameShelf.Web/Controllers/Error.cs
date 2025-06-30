using Microsoft.AspNetCore.Mvc;

namespace MyGameShelf.Web.Controllers;
public class ErrorController : Controller
{
    [Route("Error/NotFound")]
    public IActionResult NotFoundPage()
    {
        ViewBag.Message = "The page you're looking for doesn't exist.";
        ViewBag.RedirectText = "Return Home";
        ViewBag.RedirectUrl = Url.Action("Index", "Home");

        return View("NotFound");
    }
}
