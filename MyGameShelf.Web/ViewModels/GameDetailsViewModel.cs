using MyGameShelf.Application.DTOs;

namespace MyGameShelf.Web.ViewModels;

public class GameDetailsViewModel
{
    public GameDetailDto Game { get; set; }

    public string? PublisherIdsString { get; set; }
    public bool HasRelatedGames { get; set; }
}
