using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Enums;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Data;
using MyGameShelf.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Services;
public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserGameWithFavoriteStatus>> GetUserGamesAsync(string userId, string? status, string? sort, int page = 1, int pageSize = 10)
    {
        return await _unitOfWork.UserGames.GetUserGamesAsync(userId, status, sort, page, pageSize);
    }
    
    public async Task<Game?> AddGameMetadataAsync(Game incomingGame)
    {
        try
        {
            // Check if game with same RawgId already exists
            var existingGame = await _unitOfWork.Games.GetByRawgIdAsync(incomingGame.RawgId);

            if (existingGame != null)
            {
                return existingGame; // Don't add a duplicate
            }

            var platforms = await _unitOfWork.Platforms.GetAllAsync();

            // Attach existing Platforms by matching name (case-insensitive)
            foreach (var gamePlatform in incomingGame.Platforms)
            {
                var existingPlatform = platforms.FirstOrDefault(p => p.Name.ToLower() == gamePlatform.Platform.Name.ToLower());

                gamePlatform.Platform = existingPlatform ?? gamePlatform.Platform;
            }

            var genres = await _unitOfWork.Genres.GetAllAsync();

            // Attach existing Genres
            foreach (var gameGenre in incomingGame.GameGenres)
            {
                var existingGenre = genres.FirstOrDefault(g => g.Name.ToLower() == gameGenre.Genre.Name.ToLower());

                gameGenre.Genre = existingGenre ?? gameGenre.Genre;
            }

            var tags = await _unitOfWork.Tags.GetAllAsync();

            // Attach existing Tags
            foreach (var gameTag in incomingGame.GameTags)
            {
                var existingTag = tags.FirstOrDefault(t => t.Name.ToLower() == gameTag.Tag.Name.ToLower());

                gameTag.Tag = existingTag ?? gameTag.Tag;
            }

            var developers = await _unitOfWork.Developers.GetAllAsync();

            // Attach existing Developers
            foreach (var gameDev in incomingGame.GameDevelopers)
            {
                var existingDev = developers.FirstOrDefault(d => d.Name.ToLower() == gameDev.Developer.Name.ToLower());

                gameDev.Developer = existingDev ?? gameDev.Developer;
            }

            var publishers = await _unitOfWork.Publishers.GetAllAsync();

            // Attach existing Publishers
            foreach (var gamePub in incomingGame.GamePublishers)
            {
                var existingPub = publishers.FirstOrDefault(p => p.Name.ToLower() == gamePub.Publisher.Name.ToLower());

                gamePub.Publisher = existingPub ?? gamePub.Publisher;
            }

            // Add the new game entity with all its relations (attached or new)
            await _unitOfWork.Games.AddGameAsync(incomingGame);

            var result = await _unitOfWork.SaveChangesAsync();

            if (!result)
            {
                return null;
            }


            return incomingGame;
        }
        catch (Exception ex)
        {
            //throw new Exception("Failed to add game", ex);
            return null;
        }
    }
    
    public async Task<Dictionary<GameStatus, List<UserGameDto>>> GetUserGameListAsync(string userId)
    {
        var userGames = await _unitOfWork.Games.GetUserGamesAsync(userId);

        var dtos = userGames.Select(ug => new UserGameDto
        {
            GameId = ug.Game.Id,
            Name = ug.Game.Name,
            BackgroundImage = ug.Game.BackgroundImage,
            Status = ug.Status,
            Difficulty = ug.Difficulty,
            AddedOn = ug.AddedOn,
            Genres = ug.Game.GameGenres.Select(gg => gg.Genre.Name).ToList(),
            Platforms = ug.Game.Platforms.Select(gp => gp.Platform.Name).ToList(),
            Tags = ug.Game.GameTags.Select(gt => gt.Tag.Name).ToList()
        }).ToList();

        return dtos
            .GroupBy(dto => dto.Status)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
    
    public async Task<UserGameDetailsDto?> GetUserGameDetailsAsync(string userId, int rawgId)
    {
        // Get the UserGame along with the Game (to check RawgId)
        var userGame = await _unitOfWork.UserGames.GetUserGameAsync(userId, rawgId);

        if (userGame == null)
        { 
            return null;
        }

        // Now separately query the Review
        var review = await _unitOfWork.Reviews.GetReviewAsync(userId, userGame.GameId);

        return new UserGameDetailsDto
        {
            Status = userGame.Status,
            Rating = userGame.Rating,
            Difficulty = userGame.Difficulty,
            ReviewContent = review?.Content,
            IsRecommended = review?.IsRecommended,
            ReviewUpdatedAt = review?.UpdatedAt
        };
    }

    public async Task<int> CountGamesByStatusAsync(string userId, string? status)
    {
        return await _unitOfWork.UserGames.CountByStatusAsync(userId, status);
    }

    public async Task<bool> AddGameToUserAsync(string userId, int gameId, GameStatus status, double? difficulty, string? review, double? rating, bool? recommended)
    {
        try
        {
            var userGame = new UserGame(userId, gameId)
            {
                Status = status,
            };

            userGame.SetDifficulty(difficulty);
            userGame.SetRating(rating);

            await _unitOfWork.UserGames.AddUserGame(userGame);

            // Add review if provided
            if (!string.IsNullOrWhiteSpace(review))
            {

                if (!recommended.HasValue)
                {
                    // Throw an error because recommendation is missing but review exists
                    throw new InvalidOperationException("Recommendation must be specified when submitting a review.");
                }

                var reviewEntity = new Review(userId, gameId, review, recommended.Value);
                await _unitOfWork.Reviews.AddReview(reviewEntity);
            }

            return await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex) // Exception to catch duplicate Games in User Games
        {
            if (ex.InnerException?.Message.Contains("IX_UserGames_UserId_GameId") == true)
            {
                // Unlikely due to the pre-check, but safely handle the race condition
                return false;
            }

            // Rethrow if it's a different DB error
            throw;
        }
        catch (Exception)
        { 
            return false; 
        }
    }

    public async Task<bool> UserHasGameAsync(string userId, int rawgId)
    {
        return await _unitOfWork.UserGames.CheckUserGameExists(userId, rawgId);
    }

    public async Task<bool> RemoveGameFromUserAsync(string userId, int gameId)
    {
        return await _unitOfWork.UserGames.RemoveUserGameAsync(userId, gameId);
    }


    public async Task<UserGameWithReviewDto?> GetUserGameWithReviewAsync(string userId, int rawgId)
    {
        // Get the UserGame along with the Game (to check RawgId)
        var userGame = await _unitOfWork.UserGames.GetUserGameAsync(userId, rawgId);

        if (userGame == null)
        { 
            return null;
        }

        // Now separately query the Review
        var review = await _unitOfWork.Reviews.GetReviewAsync(userId, userGame.GameId);

        return new UserGameWithReviewDto
        {
            UserGame = userGame,
            Review = review
        };
    }

    public async Task<bool> UpdateOrAddReviewAsync(string userId, int gameId, string? reviewContent, bool isRecommended)
    {
        try
        {
            // Check if user already has a review for game
            var existingReview = await _unitOfWork.Reviews.GetReviewAsync(userId, gameId);

            // If Review content is empty - consider it a delete
            if (string.IsNullOrWhiteSpace(reviewContent))
            {
                // Review Exists - delete it
                if (existingReview != null)
                {
                    await _unitOfWork.Reviews.DeleteReview(existingReview);
                    return await _unitOfWork.SaveChangesAsync();
                }

                // Nothing to delete - consider success
                return true; 
            }

            // Review Exists
            if (existingReview != null)
            {
                // update review
                existingReview.UpdateReview(reviewContent, isRecommended);
            }
            else
            {
                // Create new Review
                var newReview = new Review(userId, gameId, reviewContent, isRecommended);
                await _unitOfWork.Reviews.AddReview(newReview);
            }

            return await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Log exception if you have logging
            return false;
        }
    }

    public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId, string? sort, int page = 1, int pageSize = 10)
    {
        return await _unitOfWork.Reviews.GetUserReviewsAsync(userId, sort, page, pageSize);
    }

    public async Task<int> CountUserReviewsAsync(string userId)
    {
        return await _unitOfWork.Reviews.CountUserReviewsAsync(userId);
    }

    public async Task<bool> DeleteReviewAsync(string userId, int reviewId)
    {
        await _unitOfWork.Reviews.DeleteReviewAsync(userId, reviewId);
        return await _unitOfWork.SaveChangesAsync();
    }


    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId, string? sort, int page = 1, int pageSize = 10)
    {
        return await _unitOfWork.Favorites.GetUserFavoritesAsync(userId, sort, page, pageSize);
    }

    public async Task<int> CountUserFavoritesAsync(string userId)
    {
        return await _unitOfWork.Favorites.CountUserFavoritesAsync(userId);
    }

    public async Task ToggleFavoriteGameAsync(string userId, int gameId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var existingFavorite = await _unitOfWork.Favorites.GetByUserAndGameAsync(userId, gameId);

        // Optional cooldown (e.g., 1 second between toggles)
        if (existingFavorite != null)
        {
            var now = DateTime.UtcNow;
            var timeSinceLastToggle = now - existingFavorite.FavoritedOn;

            if (timeSinceLastToggle.TotalSeconds < 1)
                return; // Or throw, or just silently ignore

            await _unitOfWork.Favorites.DeleteFavorite(existingFavorite);
        }
        else
        {
            var favorite = new Favorite(userId, gameId);
            await _unitOfWork.Favorites.AddFavorite(favorite);
        }

        await _unitOfWork.SaveChangesAsync();
    }

}
