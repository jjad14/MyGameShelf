using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Web.ViewModels;

namespace MyGameShelf.Web.Controllers;

[Authorize]
[Route("profile")]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet("/profile/{username}")]
    public async Task<IActionResult> Index(string username)
    {
        if (string.IsNullOrEmpty(username))
        { 
            return NotFound();
        }

        var user = await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
        { 
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ProfileMessage = user.ProfileMessage,
            Gender = user.Gender,
            Birthday = user.Birthday,
            Address = user.Address,
            XSocialLink = user.XSocialLink,
            InstagramSocialLink = user.InstagramSocialLink,
            FacebookSocialLink = user.FacebookSocialLink,
            YoutubeSocialLink = user.YoutubeSocialLink,
            TwitchSocialLink = user.TwitchSocialLink,
            CreatedAt = user.CreatedAt,
            LastActive = user.LastActive
        };

        return View(model);
    }


    [HttpGet]
    public IActionResult Settings() 
    { 
        return View();
    }
}
