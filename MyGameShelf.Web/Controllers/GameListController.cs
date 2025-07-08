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
using System.Globalization;

namespace MyGameShelf.Web.Controllers;

[Authorize]
public class GameListController : BaseController
{
    private readonly IGameService _gameService;
    private readonly IRawgApiService _rawgApiService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GameListController(IGameService gameService, IRawgApiService rawgApiService, UserManager<ApplicationUser> userManager, 
        ILogger<BaseController> logger) : base(userManager, logger)
    {
        _gameService = gameService;
        _userManager = userManager;
        _rawgApiService = rawgApiService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(GameDetailsViewModel model)
    {
        try
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
        }
        catch (Exception)
        {
            TempData["error"] = "An error occurred while adding the game to your list. Please try again.";

             return RedirectToAction("Details", "Games", new { id = model.AddToList.GameId });
            // return RedirectToAction("Index", "Games");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(GameDetailsViewModel model)
    {
        try
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
        catch (Exception)
        {
            TempData["error"] = "An error occurred while updating the game to your list. Please try again.";

            return RedirectToAction("Details", "Games", new { id = model.AddToList.GameId });
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveGame(string userId, int gameId)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.Id != userId)
            {
                return Unauthorized();
            }

            var removed = await _gameService.RemoveGameFromUserAsync(userId, gameId);

            TempData[removed ? "success" : "error"] = removed
                ? "Game successfully removed from your list."
                : "Failed to remove the game from your list.";

            return Ok(new { success = removed });
        }
        catch (Exception)
        {
            TempData["error"] = "An error occurred while removing the game from your list. Please try again.";
            return BadRequest(new { success = false });
        }
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> RemoveReview(int reviewId)
    //{
    //    try
    //    {
    //        var user = await _userManager.GetUserAsync(User);
    //        if (user == null)
    //        {
    //            return Unauthorized();
    //        }
    //        var removed = await _gameService.RemoveGameFromUserAsync(user.Id, gameId);


    //        TempData[removed ? "success" : "error"] = removed
    //            ? "Game successfully removed from your list."
    //            : "Failed to remove the game from your list.";
    //        return Ok(new { success = removed });
    //    }
    //    catch (Exception)
    //    {
    //        TempData["error"] = "An error occurred while removing the game from your list. Please try again.";
    //        return BadRequest(new { success = false });
    //    }
    //}


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavoriteGame(int gameId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            await _gameService.ToggleFavoriteGameAsync(user.Id, gameId);


            TempData["success"] = "Game favorite status updated successfully.";

            return Ok(new { success = true });
        }
        catch (Exception)
        {
            TempData["error"] = "An error occurred while updating the game to your list. Please try again.";

            return BadRequest(new { success = false });
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UserGamesByFilter(string userId, string? status, string? sort, int page = 1)
    {
        var isOwner = IsCurrentUser(userId);

        try
        {
            const int pageSize = 10;

            var games = await _gameService.GetUserGamesAsync(userId, status, sort, page, pageSize);

            var totalCount = await _gameService.CountGamesByStatusAsync(userId, status);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var viewModel = new UserGamesTabViewModel
            {
                GamesWithFavorites = games,
                CurrentStatus = status ?? "All",
                CurrentPage = page,
                TotalPages = totalPages,
                UserId = userId,
                IsOwner = isOwner
            };

            return PartialView("_UserGamesTab", viewModel);
        }
        catch (Exception)
        {
            var viewModel = new UserGamesTabViewModel
            {
                GamesWithFavorites = Enumerable.Empty<UserGameWithFavoriteStatus>().ToList(),
                CurrentStatus = status ?? "All",
                CurrentPage = page,
                TotalPages = 1,
                UserId = userId,
                IsOwner = isOwner
            };

            return PartialView("_UserGamesTab", viewModel);
        }

    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UserFavorites(string userId, string? sort, int page = 1)
    {
        var isOwner = IsCurrentUser(userId);

        try
        {
            const int pageSize = 10;

            var favorites = await _gameService.GetUserFavoritesAsync(userId, sort, page, pageSize);

            var totalCount = await _gameService.CountUserFavoritesAsync(userId);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Map domain Review -> UserReviewViewModel
            var result = favorites.Select(r => new UserFavoritesViewModel
            {
                FavoriteId = r.Id,
                GameId = r.Game.RawgId,
                GameTitle = r.Game.Name,
                GameImageUrl = r.Game.BackgroundImage ?? "",
                Metacritic = r.Game.Metacritic,
                EsrbRating = r.Game.EsrbRating,
                CreatedAt = r.CreatedAt
            }).ToList();

            var viewModel = new UserFavoritesTabViewModel
            {
                UserId = userId,
                IsOwner = isOwner,
                Favorites = result,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return PartialView("_UserFavoritesTab", viewModel);
        }
        catch (Exception)
        {
            var viewModel = new UserFavoritesTabViewModel
            {
                UserId = userId,
                IsOwner = isOwner,
                Favorites = Enumerable.Empty<UserFavoritesViewModel>().ToList(),
                CurrentPage = page,
                TotalPages = 1
            };

            return PartialView("_UserFavoritesTab", viewModel);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UserReviews(string userId, string? sort, int page = 1)
    {
        var isOwner = IsCurrentUser(userId);
        
        try
        {
            const int pageSize = 10;

            var reviews = await _gameService.GetUserReviewsAsync(userId, sort, page, pageSize);

            var totalCount = await _gameService.CountUserReviewsAsync(userId);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Map domain Review -> UserReviewViewModel
            var result = reviews.Select(r => new UserReviewViewModel
            {
                ReviewId = r.Id,
                GameId = r.Game.RawgId,
                GameTitle = r.Game.Name,
                GameImageUrl = r.Game.BackgroundImage ?? "",
                Content = r.Content,
                IsRecommended = r.IsRecommended,
                CreatedAt = r.CreatedAt
            }).ToList();

            var viewModel = new UserReviewsTabViewModel
            {
                UserId = userId,
                IsOwner = isOwner,
                Reviews = result,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return PartialView("_UserReviewsTab", viewModel);
        }
        catch (Exception)
        {
            var viewModel = new UserReviewsTabViewModel
            {
                UserId = userId,
                IsOwner = isOwner,
                Reviews = Enumerable.Empty<UserReviewViewModel>().ToList(),
                CurrentPage = page,
                TotalPages = 1
            };

            return PartialView("_UserReviewsTab", viewModel);
        }
    }
}
