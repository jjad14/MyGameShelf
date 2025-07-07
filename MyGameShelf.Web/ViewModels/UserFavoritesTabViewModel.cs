namespace MyGameShelf.Web.ViewModels;

public class UserFavoritesTabViewModel
{
    public string UserId { get; set; } = null!;
    public bool IsOwner { get; set; }
    public List<UserFavoritesViewModel> Favorites { get; set; } = new();
}
