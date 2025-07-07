namespace MyGameShelf.Web.ViewModels;

public class UserFavoritesViewModel
{
    public int FavoriteId { get; set; }
    public string GameTitle { get; set; }
    public string? GameImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int GameId { get; set; }
    public int? Metacritic { get; set; }
    public string? EsrbRating { get; set; }

}
