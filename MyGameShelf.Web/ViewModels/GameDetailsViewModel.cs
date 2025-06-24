using MyGameShelf.Application.DTOs;

namespace MyGameShelf.Web.ViewModels;

public class GameDetailsViewModel
{
    public GameDetailDto Game { get; set; }

    public string? PublisherIdsString { get; set; }
    public bool HasRelatedGames { get; set; }
    public bool HasSequels { get; set; }
    public bool HasAdditions { get; set; }
}
