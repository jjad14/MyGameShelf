namespace MyGameShelf.Web.ViewModels;

public class SettingsCheck
{
    public bool IsPublic { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool HasPassword { get; set; }
    public bool IsExternalLogin { get; set; }
}
