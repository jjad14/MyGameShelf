using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Web.ViewModels;
public class LoginViewModel
{
    [Required]
    [Display(Name = "Email or Username")]
    public string EmailOrUsername { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
