using CloudinaryDotNet;
using MyGameShelf.Application.Validation.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyGameShelf.Web.ViewModels;

public class EditProfileViewModel
{
    [Required]
    [MaxLength(50)]
    [DisplayName("First Name")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    [DisplayName("Last Name")]
    public string LastName { get; set; }

    [MaxLength(250)]
    [DisplayName("Profile Message")]
    public string? ProfileMessage { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public IFormFile? NewProfilePicture { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [BirthdayRange]
    public DateTime Birthday { get; set; }

    [Required]
    public string Street { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string Province { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public string Country { get; set; }

    [Url]
    public string? XSocialLink { get; set; }

    [Url]
    public string? InstagramSocialLink { get; set; }

    [Url]
    public string? FacebookSocialLink { get; set; }

    [Url]
    public string? YoutubeSocialLink { get; set; }

    [Url]
    public string? TwitchSocialLink { get; set; }

    // Account Settings

    public bool IsPublic { get; set; }

    public bool TwoFactorEnabled { get; set; }


    [DataType(DataType.Password)]
    [DisplayName("Current Password")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [DisplayName("New Password")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [DisplayName("Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmNewPassword { get; set; }


    // Props to check if user was signed in locally or google auth
    public bool HasPassword { get; set; }
    public bool IsExternalLogin { get; set; }
}
