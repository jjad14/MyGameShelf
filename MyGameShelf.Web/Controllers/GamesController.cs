using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Application.Interfaces;

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
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        var games = await _rawgApiService.GetPopularGamesAsync(page, pageSize);
        return View(games);
    }
}
