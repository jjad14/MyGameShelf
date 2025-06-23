using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public async Task<IActionResult> Index(string search = null, string platform = null, string genre = null,
                                           string metacritic = null, string orderBy = null, int page = 1, int pageSize = 20)
    {
        string developer = null;
        string publisher = null;

        // Make sure these calls return a List<SelectListItem>
        var platforms = await _rawgApiService.GetPlatformsAsync();
        ViewBag.Platforms = platforms.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Name
        }).ToList();

        // Make sure these calls return a List<SelectListItem>
        var genres = await _rawgApiService.GetGenresAsync();
        ViewBag.Genres = genres.Select(g => new SelectListItem
        {
            Value = g.Id.ToString(),
            Text = g.Name
        }).ToList();


        var response = await _rawgApiService.GetGamesBySearchAndFilters(
            search, platform, developer, publisher, genre, metacritic, orderBy, page, pageSize);

        var gamesVm = new PaginatedGamesViewModel
        {
            Games = response.Games,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(response.TotalCount / (double)pageSize)
        };
       
        return View(gamesVm);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(int id)
    { 
        var response = await _rawgApiService.GetGameDetailsAsync(id);

        var gameDetailsVM = new GameDetailsViewModel
        { 
            Game = response
        };

        return View(gameDetailsVM);
    }



}
