using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Web.ViewModels;

namespace MyGameShelf.Web.Controllers;

[Authorize]
[Route("profile")]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPhotoService _photoService;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IPhotoService photoService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _photoService = photoService;
    }

    [AllowAnonymous]
    [HttpGet("profile/{username:regex(^[[a-zA-Z0-9_-]]+$)}")]
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
            LastActive = user.LastActive,
            IsPublic = user.IsPublic,
        };

        return View(model);
    }


    [HttpGet("settings")]
    public async Task<IActionResult> Settings() 
    {
        var user = await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null || user.Address == null)
        {
            return NotFound();
        }

        // Get EditProfileViewModel with all properties from user
        var model = await LoadEditProfileViewModel(user);

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileSettings(EditProfileViewModel editProfileViewModel)
    {
        if (!ModelState.IsValid)
        {
            // Return view with validation errors
            return View(editProfileViewModel);
        }

        // Get the current user with related Address navigation property
        var user = await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null)
        {
            return NotFound();
        }

        // Update profile picture if new one uploaded
        if (editProfileViewModel.NewProfilePicture != null)
        {
            // Delete old picture using publicId
            if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
            {
                await _photoService.DeletePhotoAsync(user.ProfilePicturePublicId);
            }

            // Upload new photo
            var uploadResult = await _photoService.AddPhotoAsync(editProfileViewModel.NewProfilePicture);
            user.ProfilePictureUrl = uploadResult.Url;
            user.ProfilePicturePublicId = uploadResult.PublicId;
        }

        // Update other user fields
        user.FirstName = editProfileViewModel.FirstName;
        user.LastName = editProfileViewModel.LastName;
        user.ProfileMessage = editProfileViewModel.ProfileMessage;
        user.Gender = editProfileViewModel.Gender;
        user.Birthday = editProfileViewModel.Birthday;

        // Update Address
        user.Address.Street = editProfileViewModel.Street;
        user.Address.City = editProfileViewModel.City;
        user.Address.Province = editProfileViewModel.Province;
        user.Address.PostalCode = editProfileViewModel.PostalCode;
        user.Address.Country = editProfileViewModel.Country;

        // Update social links
        user.XSocialLink = editProfileViewModel.XSocialLink;
        user.InstagramSocialLink = editProfileViewModel.InstagramSocialLink;
        user.FacebookSocialLink = editProfileViewModel.FacebookSocialLink;
        user.YoutubeSocialLink = editProfileViewModel.YoutubeSocialLink;
        user.TwitchSocialLink = editProfileViewModel.TwitchSocialLink;

        // Update User
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(editProfileViewModel);
        }

        // Set a success message (TempData, ViewData, etc.)
        TempData["SuccessMessage"] = "Profile updated successfully.";

        // Redirect back to profile page
        //return RedirectToAction("Index", new { username = user.UserName });
        return RedirectToAction("Settings");

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAccountSettings(EditProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        // Update profile visibility
        user.IsPublic = model.IsPublic;

        // Change password if provided
        if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
        {
            var changePassResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!changePassResult.Succeeded)
            {
                foreach (var error in changePassResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Settings", model);
            }
        }

        // Update User
        var updateResult = await _userManager.UpdateAsync(user);

        // Check if update succeded
        if (!updateResult.Succeeded)
        {
            // Return Model State errors
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Settings", model);
        }

        TempData["SuccessMessage"] = "Account settings updated successfully.";

        return RedirectToAction("Settings");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        // Save profile picture ID
        var profileId = user.ProfilePicturePublicId;
        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Settings", await LoadEditProfileViewModel(user));
        }

        // Delete Profile Picture
        await _photoService.DeletePhotoAsync(profileId);

        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleTwoFactorAuthentication(string actionType) // actionType = "enable" or "disable"
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        // If action type is enable, we enable 2FA
        if (actionType == "enable")
        {
            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Two-Factor Authentication enabled.";

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        } // If action type is disable, we disable 2FA
        else if (actionType == "disable")
        {
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Two-Factor Authentication disabled.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
        } // If action type is invalid, throw model state error
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid action.");
        }

        if (!ModelState.IsValid)
        {
            return View("Settings", await LoadEditProfileViewModel(user));
        }

        return RedirectToAction("Settings");
    }


    [HttpGet("setup2fa")]
    public async Task<IActionResult> Setup2FA()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        // Generate Authenticator Key
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var email = await _userManager.GetEmailAsync(user);

        // Format Key
        var sharedKey = FormatKey(unformattedKey);

        // Generate QR Code using user email and Authenticator Key
        var authenticatorUri = GenerateQrCodeUri(email, unformattedKey);

        // Create View Model
        var model = new TwoFactorSetupViewModel
        {
            SharedKey = sharedKey,
            AuthenticatorUri = authenticatorUri
        };

        return View(model);
    }

    [HttpPost("setup2fa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup2FA(TwoFactorSetupViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Always re-populate these, because they're not posted from the form
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        model.SharedKey = FormatKey(unformattedKey);
        model.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Verify Authenticator Code
        var is2FATokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.VerificationCode);

        if (!is2FATokenValid)
        {
            ModelState.AddModelError(nameof(model.VerificationCode), "Invalid verification code.");
            return View(model);
        }

        // Set 2FA to enabled
        await _userManager.SetTwoFactorEnabledAsync(user, true);

        TempData["SuccessMessage"] = "Two-Factor Authentication has been enabled.";

        return RedirectToAction("Settings");
    }

    private async Task<EditProfileViewModel> LoadEditProfileViewModel(ApplicationUser user)
    {
        // Using User, create a Profile View Model
        return new EditProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileMessage = user.ProfileMessage,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Gender = user.Gender,
            Birthday = user.Birthday,
            Street = user.Address.Street,
            City = user.Address.City,
            Province = user.Address.Province,
            PostalCode = user.Address.PostalCode,
            Country = user.Address.Country,
            XSocialLink = user.XSocialLink,
            InstagramSocialLink = user.InstagramSocialLink,
            FacebookSocialLink = user.FacebookSocialLink,
            YoutubeSocialLink = user.YoutubeSocialLink,
            TwitchSocialLink = user.TwitchSocialLink,
            IsPublic = user.IsPublic,
            TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
        };
    }


    private string FormatKey(string unformattedKey)
    {
        // Format key 4 character spaced
        return string.Join(" ", Enumerable.Range(0, unformattedKey.Length / 4)
            .Select(i => unformattedKey.Substring(i * 4, 4)));
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(unformattedKey))
            return string.Empty;

        // Return generated QR Code
        return string.Format(
            "otpauth://totp/{0}?secret={1}&issuer={2}&digits=6",
            Uri.EscapeDataString("MyGameShelf:" + email),
            unformattedKey,
            Uri.EscapeDataString("MyGameShelf"));
    }

}
