namespace MyGameShelf.Web.ViewModels;

public class UserReviewViewModel
{
    public int ReviewId { get; set; }
    public string GameTitle { get; set; }
    public string? GameImageUrl { get; set; }
    public string Content { get; set; }
    public bool? IsRecommended { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int GameId { get; set; }
    public bool CanEdit { get; set; }
}
