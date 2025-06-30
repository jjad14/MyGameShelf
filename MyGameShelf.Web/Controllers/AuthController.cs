using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Exceptions;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Web.ViewModels;
using System.Security.Claims;

namespace MyGameShelf.Web.Controllers;
public class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPhotoService _photoService;

    public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IPhotoService photoService, ILogger<BaseController> logger) : base(logger)
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
        try
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            // Check if user already exists by email
            if (await _userManager.FindByEmailAsync(registerViewModel.Email) != null)
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                return View(registerViewModel);
            }

            // Check if user already exists by username
            if (await _userManager.FindByNameAsync(registerViewModel.UserName) != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already taken.");
                return View(registerViewModel);
            }

            // Photo info
            string? photoUrl = null;
            string? photoUrlId = null;
            PhotoUploadResult? uploadResult = null;

            // Upload profile picture if provided
            if (registerViewModel.ProfilePicture != null && registerViewModel.ProfilePicture.Length > 0)
            {
                // Add photo
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

            // Create User
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            // If creation fails return error messages
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
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, "Network error occurred during photo upload.");
            return View(registerViewModel);
        }
        catch (IOException ex)
        {
            ModelState.AddModelError(string.Empty, "Error reading the uploaded photo.");
            return View(registerViewModel);
        }
        catch (PhotoServiceException ex)
        {
            ModelState.AddModelError(string.Empty, "An unexpected error occurred during photo upload.");

            return View(registerViewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration.");

            return View(registerViewModel);
        }

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
        try
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

            // If no user exists return error message
            if (user == null) 
            {
                TempData["ErrorMessage"] = "Invalid login attempt.";
                return RedirectToAction("Login");
            }

            // Sign in via username and passwod
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);

            // Check if user's email is not confirmed, if no then it wont be allowed
            if (result.IsNotAllowed)
            {
                // User email is not confirmed
                TempData["ErrorMessage"] = "You need to confirm your email before you can log in.";
                return RedirectToAction("Login");
            }

            // Check if User has 2FA enabled
            if (result.RequiresTwoFactor)
            {
                // Redirect to 2FA verification page with returnUrl and RememberMe info
                return RedirectToAction("LoginWith2fa", new { returnUrl, loginViewModel.RememberMe });
            }

            // Check if User is locked out
            if (result.IsLockedOut)
            {
                // Handle lockout case (optional)
                TempData["ErrorMessage"] = "User account locked out.";
                return RedirectToAction("Login");
            }

            // Check if login succeded
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Invalid login attempt.";
                return RedirectToAction("Login");
            }

            // Check if there is a redirectURL - if so use this instead of the default redirection
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Games");

        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = "Network error occurred during photo upload.";

            return RedirectToAction("Login");
        }
        catch (IOException ex)
        {
            TempData["ErrorMessage"] = "Error reading the uploaded photo.";
            
            return RedirectToAction("Login");
        }
        catch (PhotoServiceException ex)
        {
            TempData["ErrorMessage"] = "User Registration Failed.";
            
            return RedirectToAction("Login");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "User Login Failed.";

            return RedirectToAction("Login");
        }

    }

    [HttpGet]
    public IActionResult LoginWith2fa(string returnUrl = null, bool rememberMe = false)
    {
        var model = new LoginWith2faViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // sign in via 2FA, requires two factor code
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            model.TwoFactorCode, model.RememberMe, rememberClient: model.RememberMachine);

        // if user is locked out
        if (result.IsLockedOut)
        {
            TempData["ErrorMessage"] = "User account locked out.";
            return RedirectToAction("Login");
        }

        // Check if 2FA authentication fails
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return View(model);
        }

        // Check for redirect URL
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Games");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            return RedirectToAction("Login", new { ErrorMessage = $"Error from external provider: {remoteError}" });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction("Login", new { ErrorMessage = "Error loading external login info." });
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }

        // Extract data from external provider (Google)
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var picture = info.Principal.FindFirstValue("picture");

        // Check if the user already exists by email
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            // Link login if needed
            await _userManager.AddLoginAsync(existingUser, info);
            await _signInManager.SignInAsync(existingUser, isPersistent: false);
            return RedirectToAction("Index", "Games");
        }

        // Need to create a user name using the email, since email syntax is not a valid username (@symbol)
        var baseUsername = email?.Split('@')[0] ?? "user";
        var username = await GenerateUniqueUsername(baseUsername);

        // Create new ApplicationUser
        var user = new ApplicationUser
        {
            UserName = username,
            Email = email,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            ProfilePictureUrl = picture,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            Address = new Address(), // Prevents null issues
            Gender = "Not Specified",
            Birthday = DateTime.MinValue
        };


        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded) 
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("Login");
        }

        await _userManager.AddToRoleAsync(user, UserRoles.User);
        await _userManager.AddLoginAsync(user, info);

        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToLocal(returnUrl);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // log user out and redirect to Home
        await _signInManager.SignOutAsync();

        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        return RedirectToAction("Index", "Home");
    }


    private async Task<string> GenerateUniqueUsername(string baseUsername)
    {
        string username = baseUsername;
        int suffix = 1;

        while (await _userManager.FindByNameAsync(username) != null)
        {
            username = $"{baseUsername}{suffix}";
            suffix++;
        }

        return username;
    }

}
