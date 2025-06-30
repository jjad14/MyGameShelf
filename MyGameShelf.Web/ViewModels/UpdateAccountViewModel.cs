using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyGameShelf.Web.ViewModels;

public class UpdateAccountViewModel
{
    public bool IsPublic { get; set; }

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
}
