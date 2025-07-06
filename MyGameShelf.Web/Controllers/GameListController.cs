using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Enums;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Infrastructure.Repositories;
using MyGameShelf.Web.ViewModels;

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
    public async Task<IActionResult> Add(GameDetailsViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return Unauthorized();
        

        // Get Game by Id
        var response = await _rawgApiService.GetGameDetailsAsync(model.AddToList.GameId);

        // No Game Found Case
        if (response == null) return NotFound();
        

        // Map Game data to your Game entity
        var game = await _gameService.AddGameMetadataAsync(new Game
        {
            RawgId = model.AddToList.GameId,
            Name = response.Name,
            Slug = response.Slug,
            Released = response.Released,
            BackgroundImage = response.BackgroundImage,
            Metacritic = response.Metacritic,
            Rating = response.Rating,
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

        if (!Enum.TryParse<GameStatus>(model.AddToList.GameStatus, true, out var status))
        {
            status = GameStatus.Playing;
        }

        var added = await _gameService.AddGameToUserAsync(
            user.Id,
            game.Id,
            status,
            model.AddToList.Difficulty,
            model.AddToList.Review,
            model.AddToList.Rating, 
            model.AddToList.IsRecommended
        );

        TempData[added ? "success" : "error"] = added
            ? "Game successfully added to your list!"
            : "This game is already in your list.";

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
            hasOtherGames = await _rawgApiService.HasOtherGamesByPublisher(publisherIdString, game.Id);
        }

        // Check if Game has additions (DLCs) or Prequels/Sequels
        bool hasAdditions = await _rawgApiService.HasGameDLCs(game.Id);
        bool hasSequels = await _rawgApiService.HasGameSequels(game.Id);

        var gameDetailsVM = new GameDetailsViewModel
        {
            Game = response,
            PublisherIdsString = publisherIdString,
            HasRelatedGames = hasOtherGames,
            HasAdditions = hasAdditions,
            HasSequels = hasSequels,
            AddToList = model.AddToList
        };

        return View("~/Views/Games/Details.cshtml", gameDetailsVM);


        // return RedirectToAction("Details", "Games", new { id = model.GameId });
        // return RedirectToAction("Index", "Games");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(GameDetailsViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var userGame = await _gameService.GetUserGameWithReviewAsync(user.Id, model.AddToList.GameId);
        if (userGame == null)
        {
            TempData["error"] = "Game not found in your list.";
            return RedirectToAction("Details", "Games", new { id = model.AddToList.GameId });
        }

        if (!Enum.TryParse<GameStatus>(model.AddToList.GameStatus, true, out var status))
        {
            status = GameStatus.Playing;
        }

        // Update UserGame properties
        userGame.UserGame.SetDifficulty(model.AddToList.Difficulty);
        userGame.UserGame.SetRating(model.AddToList.Rating);
        userGame.UserGame.Status = status;

        // (string userId, int gameId, string? reviewContent, bool isRecommended)
        var updatedResult = await _gameService.UpdateOrAddReviewAsync(user.Id, userGame.UserGame.GameId, model.AddToList.Review, model.AddToList.IsRecommended);


        TempData[updatedResult ? "success" : "error"] = updatedResult
            ? "Game entry updated successfully."
            : "Game entry failed to update.";

        var response = await _rawgApiService.GetGameDetailsAsync(model.AddToList.GameId);

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
            hasOtherGames = await _rawgApiService.HasOtherGamesByPublisher(publisherIdString, response.Id);
        }

        // Check if Game has additions (DLCs) or Prequels/Sequels
        bool hasAdditions = await _rawgApiService.HasGameDLCs(response.Id);
        bool hasSequels = await _rawgApiService.HasGameSequels(response.Id);

        var gameDetailsVM = new GameDetailsViewModel
        {
            Game = response,
            PublisherIdsString = publisherIdString,
            HasRelatedGames = hasOtherGames,
            HasAdditions = hasAdditions,
            HasSequels = hasSequels,
            AddToList = model.AddToList
        };

        return View("~/Views/Games/Details.cshtml", gameDetailsVM);
    }

    public async Task<IActionResult> UserGamesByFilter(string userId, string? status, string? sort, int page = 1)
    {
        const int pageSize = 10;

        var games = await _gameService.GetUserGamesAsync(userId, status, sort, page, pageSize);

        var totalCount = await _gameService.CountGamesByStatusAsync(userId, status);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var viewModel = new UserGamesTabViewModel
        {
            Games = games,
            CurrentStatus = status ?? "All",
            CurrentPage = page,
            TotalPages = totalPages,
            UserId = userId
        };

        return PartialView("_UserGamesTab", viewModel);
    }



}
