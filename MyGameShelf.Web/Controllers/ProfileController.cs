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
    private readonly IPhotoService _photoService;

    public ProfileController(UserManager<ApplicationUser> userManager, IPhotoService photoService)
    {
        _userManager = userManager;
        _photoService = photoService;
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
            LastActive = user.LastActive,
            IsPublic = user.IsPublic,
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Settings() 
    {
        var user = await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null || user.Address == null)
        {
            return NotFound();
        }

        var model = new EditProfileViewModel
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

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(EditProfileViewModel editProfileViewModel)
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

        // Optionally set a success message (TempData, ViewData, etc.)
        TempData["SuccessMessage"] = "Profile updated successfully.";

        // Redirect back to profile page
        //return RedirectToAction("Index", new { username = user.UserName });
        return RedirectToAction("Settings");

    }

}
