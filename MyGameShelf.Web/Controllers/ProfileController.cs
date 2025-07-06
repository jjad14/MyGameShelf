using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Web.Helpers;
using MyGameShelf.Web.ViewModels;

namespace MyGameShelf.Web.Controllers;

[Authorize]
[Route("profile")]
public class ProfileController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPhotoService _photoService;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
        IPhotoService photoService, ILogger<BaseController> logger) : base(logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _photoService = photoService;
    }

    [AllowAnonymous]
    [HttpGet("{username:regex(^[[a-zA-Z0-9_-]]+$)}")]
    public async Task<IActionResult> Index(string username)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFoundView("User not found.", Url.Action("Index", "Games"));
            }

            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFoundView("User not found.", Url.Action("Index", "Games"));
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
                UserId = user.Id
            };

            return View(model);
        }
        catch (Exception)
        {
            return ErrorView(
                "Something went wrong while loading this profile.",
                Url.Action("Index", "Home"),
                "Back to Game List"
            );
        }

    }


    [HttpGet("settings")]
    public async Task<IActionResult> Settings() 
    {
        try
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user == null || user.Address == null)
            {
                string username = User.Identity?.Name ?? string.Empty;

                var returnUrl = Url.Action("Index", "Profile", new { username });

                return NotFoundView("User not found.", returnUrl);
            }

            // Get EditProfileViewModel with all properties from user
            var model = await LoadEditProfileViewModel(user);

            return View(model);
        }
        catch (Exception)
        {
            string username = User.Identity?.Name ?? string.Empty;

            var returnUrl = Url.Action("Index", "Profile", new { username }); // assumes your action is named "Index"
            return ErrorView(
                "Something went wrong while loading this profile's settings.",
                returnUrl,
                "Back to Profile"
            );
        }

    }


    [HttpPost("update-profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileSettings(SettingsPageViewModel model)
    {
        try
        {
            // Get the current user with related Address navigation property
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                return NotFoundView("User not found.", Url.Action("Index", "Games"));
            }

            ModelState.Remove("Account");
            ModelState.Remove("SettingsCheck");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid Profile Details";

                return View("Settings", await LoadEditProfileViewModel(user));
            }

            // Update profile picture if new one uploaded
            if (model.Profile.NewProfilePicture != null)
            {
                // Delete old picture using publicId
                if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
                {
                    await _photoService.DeletePhotoAsync(user.ProfilePicturePublicId);
                }

                // Upload new photo
                var uploadResult = await _photoService.AddPhotoAsync(model.Profile.NewProfilePicture);
                user.ProfilePictureUrl = uploadResult.Url;
                user.ProfilePicturePublicId = uploadResult.PublicId;
            }

            // Update other user fields
            user.FirstName = model.Profile.FirstName;
            user.LastName = model.Profile.LastName;
            user.ProfileMessage = model.Profile.ProfileMessage;
            user.Gender = model.Profile.Gender;
            user.Birthday = model.Profile.Birthday;

            // Update Address
            user.Address.Street = model.Profile.Street;
            user.Address.City = model.Profile.City;
            user.Address.Province = model.Profile.Province;
            user.Address.PostalCode = model.Profile.PostalCode;
            user.Address.Country = model.Profile.Country;

            // Update social links
            user.XSocialLink = model.Profile.XSocialLink;
            user.InstagramSocialLink = model.Profile.InstagramSocialLink;
            user.FacebookSocialLink = model.Profile.FacebookSocialLink;
            user.YoutubeSocialLink = model.Profile.YoutubeSocialLink;
            user.TwitchSocialLink = model.Profile.TwitchSocialLink;

            // Update User
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                //return View(editProfileViewModel);
                return View("Settings", await LoadEditProfileViewModel(user));
            }

            // Set a success message (TempData, ViewData, etc.)
            TempData["SuccessMessage"] = "Profile updated successfully.";

            // Redirect back to profile page
            //return RedirectToAction("Index", new { username = user.UserName });
            return RedirectToAction("Settings");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Profile failed to update.";

            return View("Settings", model);
        }
    }


    [HttpPost("update-account")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAccountSettings(SettingsPageViewModel model)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFoundView("User not found.", Url.Action("Index", "Games"));
            }

            ModelState.Remove("Profile");
            ModelState.Remove("SettingsCheck");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid Account Details";
                return View("Settings", await LoadEditProfileViewModel(user));
            }

            // Update profile visibility
            user.IsPublic = model.Account.IsPublic;

            // Change password if provided
            if (!string.IsNullOrEmpty(model.Account.CurrentPassword) && !string.IsNullOrEmpty(model.Account.NewPassword))
            {
                var changePassResult = await _userManager.ChangePasswordAsync(user, model.Account.CurrentPassword, model.Account.NewPassword);

                if (!changePassResult.Succeeded)
                {
                    foreach (var error in changePassResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View("Settings", await LoadEditProfileViewModel(user));
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

                return View("Settings", await LoadEditProfileViewModel(user));
            }

            TempData["SuccessMessage"] = "Account settings updated successfully.";

            return RedirectToAction("Settings");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Account failed to update.";

            return View("Settings", model);
        }
    }


    [HttpPost("delete-account")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFoundView("User not found.", Url.Action("Index", "Games"));
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

        // Check if user does have a profile picture
        // Is required for a user to have one, but if they log in with Google then they wont have one, so we need to check
        if (!String.IsNullOrEmpty(profileId))
        {
            // Delete Profile Picture
            await _photoService.DeletePhotoAsync(profileId);
        }

        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }


    [HttpPost("toggle-two-factor")]
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
        var sharedKey = QrCodeHelper.FormatKey(unformattedKey);

        // Generate QR Code using user email and Authenticator Key
        var authenticatorUri = QrCodeHelper.GenerateQrCodeUri(email, unformattedKey);

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
        model.SharedKey = QrCodeHelper.FormatKey(unformattedKey);
        model.AuthenticatorUri = QrCodeHelper.GenerateQrCodeUri(email, unformattedKey);

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

    private async Task<SettingsPageViewModel> LoadEditProfileViewModel(ApplicationUser user)
    {
        var logins = await _userManager.GetLoginsAsync(user);

        return new SettingsPageViewModel
        {
            Profile = new UpdateProfileViewModel
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
            },
            Account = new UpdateAccountViewModel(),
            SettingsCheck = new SettingsCheck
            {
                IsPublic = user.IsPublic,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasPassword = await _userManager.HasPasswordAsync(user),
                IsExternalLogin = logins.Any(l => l.LoginProvider != "Local")
            }
        };


        // Using User, create a Profile View Model
        //return new EditProfileViewModel
        //{
        //    FirstName = user.FirstName,
        //    LastName = user.LastName,
        //    ProfileMessage = user.ProfileMessage,
        //    ProfilePictureUrl = user.ProfilePictureUrl,
        //    Gender = user.Gender,
        //    Birthday = user.Birthday,
        //    Street = user.Address.Street,
        //    City = user.Address.City,
        //    Province = user.Address.Province,
        //    PostalCode = user.Address.PostalCode,
        //    Country = user.Address.Country,
        //    XSocialLink = user.XSocialLink,
        //    InstagramSocialLink = user.InstagramSocialLink,
        //    FacebookSocialLink = user.FacebookSocialLink,
        //    YoutubeSocialLink = user.YoutubeSocialLink,
        //    TwitchSocialLink = user.TwitchSocialLink,
        //    IsPublic = user.IsPublic,
        //    TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
        //    HasPassword = await _userManager.HasPasswordAsync(user),
        //    IsExternalLogin = logins.Any(l => l.LoginProvider != "Local")
        //};
    }


}
