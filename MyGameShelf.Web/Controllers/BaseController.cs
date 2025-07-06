using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MyGameShelf.Infrastructure.Identity;
using System;
using System.Security.Claims;

namespace MyGameShelf.Web.Controllers;
public abstract class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> _userManager;

    protected readonly ILogger<BaseController> Logger;

    protected BaseController(UserManager<ApplicationUser> userManager, ILogger<BaseController> logger)
    {
        Logger = logger;
        _userManager = userManager;
    }

    protected bool IsCurrentUser(string userId)
    {
        var currentUserId = _userManager.GetUserId(User);
        return string.Equals(currentUserId, userId, StringComparison.OrdinalIgnoreCase);
    }

    protected string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    protected IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Games");
    }

    // Add multiple errors to the model state from a list or dictionary:
    protected void AddModelErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
    }

    protected IActionResult NotFoundView(string message = "Resource not found", string? redirectUrl = null)
    {
        return ErrorView(
            message,
            redirectUrl ?? Url.Action("Index", "Home"),
            "Return Back"
        );
    }

    // Handle Error views
    protected IActionResult ErrorView(string errorMessage, string redirectUrl, string redirectText)
    {
        ViewBag.ErrorMessage = errorMessage;
        ViewBag.RedirectUrl = redirectUrl;
        ViewBag.RedirectText = redirectText;

        return View("Error"); // assumes Error.cshtml is in Shared or current controller's Views folder
    }

    // Set a flash message to display after redirect:
    protected void SetTempDataMessage(string message, string type = "info")
    {
        TempData["Message"] = message;
        TempData["MessageType"] = type;  // e.g., info, success, warning, danger
    }

    // Helper to safely extract and validate a return URL (to prevent open redirect):
    protected string? GetReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return returnUrl;
        return null;
    }

    // Detect if the request is an AJAX call:
    protected bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    //Return a consistent 404 result with a custom view or JSON:
    protected IActionResult HandleNotFound(string message = "Resource not found")
    {
        if (IsAjaxRequest())
        {
            return NotFound(new { error = message });
        }
        return NotFound(message); // or View("NotFound")
    }

    // --- Logging helpers ---

    protected void LogInfo(string message, params object[] args) =>
        Logger.LogInformation(message, args);

    protected void LogWarning(string message, params object[] args) =>
        Logger.LogWarning(message, args);

    protected void LogError(string message, params object[] args) =>
        Logger.LogError(message, args);

    protected void LogError(Exception ex, string message, params object[] args) =>
        Logger.LogError(ex, message, args);

    // --- User role checks ---

    protected bool UserIsInRole(string role) => User.IsInRole(role);

    protected bool UserIsAdminRole() => UserIsInRole("Admin");

    protected bool UserIsUserRole() => UserIsInRole("User");

    protected bool UserHasAnyRole(params string[] roles) =>
        roles.Any(role => User.IsInRole(role));

}