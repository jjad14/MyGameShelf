namespace MyGameShelf.Web.ViewModels;

public class UserReviewsTabViewModel
{
    public string UserId { get; set; } = null!;
    public bool IsOwner { get; set; }
    public List<UserReviewViewModel> Reviews { get; set; } = new();
}
