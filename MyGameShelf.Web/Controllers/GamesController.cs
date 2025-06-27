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
        // default search - future configuration
        string developer = null;
        string publisher = null;

        // Ensure pageSize is always between 1 and 50
        pageSize = Math.Clamp(pageSize, 1, 50);

        // Get a list of platforms for filtering
        var platforms = await _rawgApiService.GetPlatformsAsync();
        ViewBag.Platforms = platforms.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Name
        }).ToList();

        // Get a list of genres for filtering
        var genres = await _rawgApiService.GetGenresAsync();
        ViewBag.Genres = genres.Select(g => new SelectListItem
        {
            Value = g.Id.ToString(),
            Text = g.Name
        }).ToList();


        // Get Game List
        var response = await _rawgApiService.GetGamesBySearchAndFilters(
            search, platform, developer, publisher, genre, metacritic, orderBy, page, pageSize);

        // Map results of Game List to View Model
        var gamesVm = new PaginatedGamesViewModel
        {
            Games = response.Games,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(response.TotalCount / (double)pageSize)
        };
       
        return View(gamesVm);
    }

    [HttpGet("details/{id}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> Details(int id)
    {
        // Get Game by Id
        var response = await _rawgApiService.GetGameDetailsAsync(id);

        // No Game Found Case
        if (response == null) 
        { 
            return NotFound();
        }

        // Build the publisher ID string
        var publisherIds = response.Publishers?
            .Where(p => p != null)
            .Select(p => p.Id.ToString());

        // Join Publisher ids into a comma separated string
        string publisherIdString = string.Join(",", publisherIds ?? Enumerable.Empty<string>());

        bool hasOtherGames = false;

        // Check if Game Publisher(s) has other games
        if (!string.IsNullOrWhiteSpace(publisherIdString))
        {
            hasOtherGames = await _rawgApiService.HasOtherGamesByPublisher(publisherIdString, id);
        }

        // Check if Game has additions (DLCs) or Prequels/Sequels
        bool hasAdditions = await _rawgApiService.HasGameDLCs(id);
        bool hasSequels = await _rawgApiService.HasGameSequels(id);

        // Map result to View Model
        var gameDetailsVM = new GameDetailsViewModel
        { 
            Game = response,
            PublisherIdsString = publisherIdString,
            HasRelatedGames = hasOtherGames,
            HasAdditions = hasAdditions,
            HasSequels = hasSequels
        };

        return View(gameDetailsVM);
    }

    [HttpGet("publisher")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetGamesByPublisher([FromQuery] string publisherIds, [FromQuery] int? excludeId)
    {
        // Methods acts as a AjAX HttpGet request from client side

        if (string.IsNullOrWhiteSpace(publisherIds))
        {
            return BadRequest("Publisher IDs are required.");
        }

        // Get games by Publisher id(s) - excluding current game
        var games = await _rawgApiService.GetGamesByPublisher(publisherIds, excludeId);
        return Ok(games);
    }

    [HttpGet("additions")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetGameAdditions([FromQuery] int? gameId)
    {
        // Methods acts as a AjAX HttpGet request from client side

        if (!gameId.HasValue)
        {
            return BadRequest("Game ID is required.");
        }

        // Get Games DLCs by gameId
        var games = await _rawgApiService.GetGamesDLCs(gameId.Value);
        return Ok(games);
    }

    [HttpGet("sequels")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetGameSequels([FromQuery] int? gameId)
    {
        // Methods acts as a AjAX HttpGet request from client side

        if (!gameId.HasValue)
        {
            return BadRequest("Game ID is required.");
        }

        // Get Games Sequels by game Id
        var games = await _rawgApiService.GetGamesSequels(gameId.Value);
        return Ok(games);
    }

}
