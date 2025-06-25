using MyGameShelf.Application.Validation.Attributes;
using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Web.ViewModels;
public class RegisterViewModel
{
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Use letters, spaces, hyphens, and apostrophes only.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Use letters, spaces, hyphens, and apostrophes only.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfilePicture { get; set; }

    [Required]
    [BirthdayRange]
    [DataType(DataType.Date)]
    public DateTime Birthday { get; set; } = DateTime.Today;

    [Required]
    public string Gender { get; set; }

    [Display(Name = "X Link")]
    public string? XSocialLink { get; set; }

    [Display(Name = "Instagram Link")]
    public string? InstagramSocialLink { get; set; }

    [Display(Name = "Facebook Link")]
    public string? FacebookSocialLink { get; set; }

    [Display(Name = "YouTube Link")]
    public string? YoutubeSocialLink { get; set; }

    [Display(Name = "Twitch Link")]
    public string? TwitchSocialLink { get; set; }

    [Required]
    [MaxLength(50)]
    public string Street { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; }

    [Required]
    [MaxLength(50)]
    public string Province { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string Country { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Passwords must include uppercase, lowercase, and a digit.")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}