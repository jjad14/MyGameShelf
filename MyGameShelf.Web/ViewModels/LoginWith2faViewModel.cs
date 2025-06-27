using System.ComponentModel.DataAnnotations;

namespace MyGameShelf.Web.ViewModels;

public class LoginWith2faViewModel
{
    [Required]
    [Display(Name = "Authenticator code")]
    [StringLength(7, MinimumLength = 6, ErrorMessage = "The code must be 6 digits.")]
    public string TwoFactorCode { get; set; }

    [Display(Name = "Remember this machine")]
    public bool RememberMachine { get; set; }

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
