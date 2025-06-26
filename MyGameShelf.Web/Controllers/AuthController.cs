using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Web.ViewModels;

namespace MyGameShelf.Web.Controllers;
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPhotoService _photoService;

    public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IPhotoService photoService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _photoService = photoService;
    }

    public IActionResult Register()
    {
        var response = new RegisterViewModel();
        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(registerViewModel);
        }

        // Check if user already exists by email or username
        // Check if email or username already taken
        if (await _userManager.FindByEmailAsync(registerViewModel.Email) != null)
        {
            ModelState.AddModelError(string.Empty, "An account with this email already exists.");
            return View(registerViewModel);
        }

        if (await _userManager.FindByNameAsync(registerViewModel.UserName) != null)
        {
            ModelState.AddModelError(string.Empty, "Username is already taken.");
            return View(registerViewModel);
        }

        string? photoUrl = null;
        string? photoUrlId = null;
        PhotoUploadResult? uploadResult = null;

        // Upload profile picture if provided
        if (registerViewModel.ProfilePicture != null && registerViewModel.ProfilePicture.Length > 0)
        {
            uploadResult = await _photoService.AddPhotoAsync(registerViewModel.ProfilePicture);

            if (uploadResult != null)
            {
                photoUrl = uploadResult.Url;
                // Save uploadResult.PublicId if you want to delete later
                photoUrlId = uploadResult.PublicId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Photo upload failed.");
                return View(registerViewModel);
            }
        }

        // Map to ApplicationUser
        var user = new ApplicationUser
        {
            Email = registerViewModel.Email,
            UserName = registerViewModel.UserName,
            FirstName = registerViewModel.FirstName,
            LastName = registerViewModel.LastName,
            Birthday = registerViewModel.Birthday,
            Gender = registerViewModel.Gender,
            Address = new Address
            {
                Street = registerViewModel.Street,
                City = registerViewModel.City,
                Province = registerViewModel.Province,
                PostalCode = registerViewModel.PostalCode,
                Country = registerViewModel.Country
            },
            ProfilePicturePublicId = photoUrlId,
            ProfilePictureUrl = photoUrl,
            XSocialLink = registerViewModel.XSocialLink,
            InstagramSocialLink = registerViewModel.InstagramSocialLink,
            FacebookSocialLink = registerViewModel.FacebookSocialLink,
            YoutubeSocialLink = registerViewModel.YoutubeSocialLink,
            TwitchSocialLink = registerViewModel.TwitchSocialLink,
            IsPublic = true,
            CreatedAt = DateTime.Now,
            LastActive = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, registerViewModel.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerViewModel);
        }

        // Add default role
        await _userManager.AddToRoleAsync(user, UserRoles.User);

        // Sign-in user
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Games");
    }

    public IActionResult Login()
    {
        var response = new LoginViewModel();
        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            TempData["ErrorMessage"] = "Invalid login attempt.";
            //return View(loginViewModel);
            return RedirectToAction("Login");
        }

        // Find the user by email or username
        ApplicationUser? user = loginViewModel.EmailOrUsername.Contains("@")
            ? await _userManager.FindByEmailAsync(loginViewModel.EmailOrUsername)
            : await _userManager.FindByNameAsync(loginViewModel.EmailOrUsername);

        if (user == null) 
        {
            TempData["ErrorMessage"] = "Invalid login attempt.";
            return RedirectToAction("Login");
        }

        // user.UserName! means UserName will not be null as we require users to have a username when registrating 
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = "Invalid login attempt.";
            return RedirectToAction("Login");
        }

        //user.LastActive = DateTime.UtcNow;
        //await _userManager.UpdateAsync(user);


        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Games");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
