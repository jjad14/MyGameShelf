using System.ComponentModel.DataAnnotations;

namespace MyGameShelf.Web.ViewModels;

public class TwoFactorSetupViewModel
{
    public string SharedKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Verification Code")]
    public string VerificationCode { get; set; }
}