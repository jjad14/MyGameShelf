using MyGameShelf.Application.DTOs;

namespace MyGameShelf.Web.ViewModels;

public class PaginatedGamesViewModel
{
    public IEnumerable<GameDto> Games { get; set; }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
