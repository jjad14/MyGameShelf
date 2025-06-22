using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Web.ViewModels;

namespace MyGameShelf.Web.Controllers;

[Route("games")]
public class GamesController : Controller
{
    private readonly IRawgApiService _rawgApiService;

    public GamesController(IRawgApiService rawgApiService)
    {
        _rawgApiService = rawgApiService;
    }

    [HttpGet("")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        string search = null;
        string platform = null;
        string developer = null;
        string publisher = null;
        string genre = null;
        string rating = null;
        string orderBy = null;


        var response = await _rawgApiService.GetGamesBySearchAndFilters(
            search, platform, developer, publisher, genre, rating, orderBy, page);

        var gamesVm = new PaginatedGamesViewModel
        {
            Games = response.Games,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(response.TotalCount / (double)pageSize)
        };
        

        return View(gamesVm);
    }
}
