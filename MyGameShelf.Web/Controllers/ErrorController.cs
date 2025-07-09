using Microsoft.AspNetCore.Mvc;

namespace MyGameShelf.Web.Controllers;
public class ErrorController : Controller
{
    [Route("Error/NotFound")]
    public new IActionResult NotFound()
    {
        ViewBag.Message = "The page you're looking for doesn't exist.";
        ViewBag.RedirectText = "Return Home";
        ViewBag.RedirectUrl = Url.Action("Index", "Home");

        return View("NotFound");
    }

    [Route("Error/Internal")]
    public IActionResult Internal()
    {
        ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
        ViewBag.RedirectText = "Return Home";
        ViewBag.RedirectUrl = Url.Action("Index", "Home");

        return View("~/Views/Shared/Error.cshtml");
    }

    [Route("Error/Forbidden")]
    public IActionResult Forbidden()
    {
        ViewBag.Message = "You don’t have permission to access this page.";
        ViewBag.RedirectText = "Return Home";
        ViewBag.RedirectUrl = Url.Action("Index", "Home");
        return View("~/Views/Shared/Error.cshtml");
    }

    [Route("Error/Unauthorized")]
    public IActionResult UnauthorizedAccess()
    {
        ViewBag.Message = "You need to be logged in to access this page.";
        ViewBag.RedirectText = "Login";
        ViewBag.RedirectUrl = Url.Action("Login", "Account");
        return View("~/Views/Shared/Error.cshtml");
    }
}
