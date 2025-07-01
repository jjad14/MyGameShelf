using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Enums;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Identity;

namespace MyGameShelf.Web.Controllers;

[Authorize]
public class GameListController : BaseController
{
    private readonly IGameService _gameService;
    private readonly IRawgApiService _rawgApiService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GameListController(IGameService gameService, IRawgApiService rawgApiService, UserManager<ApplicationUser> userManager, 
        ILogger<BaseController> logger) : base(logger)
    {
        _gameService = gameService;
        _userManager = userManager;
        _rawgApiService = rawgApiService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddGameDto model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return Unauthorized();
        

        // Get Game by Id
        var response = await _rawgApiService.GetGameDetailsAsync(model.GameId);

        // No Game Found Case
        if (response == null) return NotFound();
        

        // Map Game data to your Game entity
        var game = await _gameService.AddGameMetadataAsync(new Game
        {
            RawgId = model.GameId,
            Name = response.Name,
            Slug = response.Slug,
            Released = response.Released,
            BackgroundImage = response.BackgroundImage,
            Rating = model.Rating,
            Metacritic = response.Metacritic,
            EsrbRating = response.EsrbRating,
            Platforms = response.Platforms
                .Select(p => new GamePlatform { Platform = new Platform { Name = p } })
                .ToList(),
            GameGenres = response.Genres
                .Select(g => new GameGenre { Genre = new Genre { Name = g } })
                .ToList(),
            GameTags = response.Tags
                .Select(t => new GameTag { Tag = new Tag { Name = t } })
                .ToList(),
            GameDevelopers = response.Developers
                .Select(d => new GameDeveloper { Developer = new Developer { Name = d } })
                .ToList(),
            GamePublishers = response.Publishers
                .Select(p => new GamePublisher { Publisher = new Publisher { Name = p.Name } })
                .ToList()
        });

        if (!Enum.TryParse<GameStatus>(model.GameStatus, true, out var status))
        {
            status = GameStatus.Playing;
        }

        var added = await _gameService.AddGameToUserAsync(
            user.Id,
            game.Id,
            status,
            model.Difficulty,
            model.Review,
            model.Rating
        );


        TempData[added ? "success" : "error"] = added
            ? "Game successfully added to your list!"
            : "This game is already in your list.";

        return RedirectToAction("Details", "Games", new { id = model.GameId });
    }

}
