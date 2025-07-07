using MyGameShelf.Application.DTOs;
using MyGameShelf.Domain.Models;

namespace MyGameShelf.Web.ViewModels;

public class UserGamesTabViewModel
{
    public IEnumerable<UserGameWithFavoriteStatus> GamesWithFavorites { get; set; }

    public string CurrentStatus { get; set; } = string.Empty;

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public string UserId { get; set; } = string.Empty;

    public bool IsOwner { get; set; }
}
