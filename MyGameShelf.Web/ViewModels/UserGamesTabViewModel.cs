using MyGameShelf.Domain.Models;

namespace MyGameShelf.Web.ViewModels;

public class UserGamesTabViewModel
{
    public IEnumerable<UserGame> Games { get; set; } = new List<UserGame>();

    public string CurrentStatus { get; set; } = string.Empty;

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public string UserId { get; set; } = string.Empty;
}
