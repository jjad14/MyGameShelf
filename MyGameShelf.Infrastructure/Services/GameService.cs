using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Enums;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Services;
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly ApplicationDbContext _context;

    public GameService(IGameRepository gameRepository, ApplicationDbContext context)
    {
        _gameRepository = gameRepository;
        _context = context;
    }

    public async Task<Game> AddGameMetadataAsync(Game incomingGame)
    {
        // Check if game with same RawgId already exists
        var existingGame = await _gameRepository.GetByRawgIdAsync(incomingGame.RawgId);
        if (existingGame != null)
        {
            return existingGame; // Don't add a duplicate
        }

        // Attach existing Platforms by matching name (case-insensitive)
        foreach (var gamePlatform in incomingGame.Platforms)
        {
            var existingPlatform = await _context.Platforms
                .FirstOrDefaultAsync(p => p.Name.ToLower() == gamePlatform.Platform.Name.ToLower());

            gamePlatform.Platform = existingPlatform ?? gamePlatform.Platform;
        }

        // Attach existing Genres
        foreach (var gameGenre in incomingGame.GameGenres)
        {
            var existingGenre = await _context.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == gameGenre.Genre.Name.ToLower());

            gameGenre.Genre = existingGenre ?? gameGenre.Genre;
        }

        // Attach existing Tags
        foreach (var gameTag in incomingGame.GameTags)
        {
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == gameTag.Tag.Name.ToLower());

            gameTag.Tag = existingTag ?? gameTag.Tag;
        }

        // Attach existing Developers
        foreach (var gameDev in incomingGame.GameDevelopers)
        {
            var existingDev = await _context.Developers
                .FirstOrDefaultAsync(d => d.Name.ToLower() == gameDev.Developer.Name.ToLower());

            gameDev.Developer = existingDev ?? gameDev.Developer;
        }

        // Attach existing Publishers
        foreach (var gamePub in incomingGame.GamePublishers)
        {
            var existingPub = await _context.Publishers
                .FirstOrDefaultAsync(p => p.Name.ToLower() == gamePub.Publisher.Name.ToLower());

            gamePub.Publisher = existingPub ?? gamePub.Publisher;
        }

        // Add the new game entity with all its relations (attached or new)
        await _gameRepository.AddGameAsync(incomingGame);
        await _gameRepository.SaveChangesAsync();

        return incomingGame;
    }

    public async Task<bool> AddGameToUserAsync(string userId, int gameId, GameStatus status, double? difficulty, string? review, double? rating, bool? recommended)
    {
        //var exists = await _context.UserGames
        //    .AnyAsync(ug => ug.UserId == userId && ug.GameId == gameId);

        //if (exists)
        //{
        //    return false;
        //}

        var userGame = new UserGame(userId, gameId)
        {
            Status = status,
        };

        userGame.SetDifficulty(difficulty);
        userGame.SetRating(rating);

        _context.UserGames.Add(userGame);

        // Add review if provided
        if (!string.IsNullOrWhiteSpace(review))
        {

            if (!recommended.HasValue)
            {
                // Throw an error because recommendation is missing but review exists
                throw new InvalidOperationException("Recommendation must be specified when submitting a review.");
            }

            var reviewEntity = new Review(userId, gameId, review, recommended.Value);
            _context.Reviews.Add(reviewEntity);
        }

        // Exception to catch duplicate Games in User Games
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_UserGames_UserId_GameId") == true)
            {
                // Unlikely due to the pre-check, but safely handle the race condition
                return false;
            }

            // Rethrow if it's a different DB error
            throw;
        }
    }

    public async Task<Dictionary<GameStatus, List<UserGameDto>>> GetUserGameListAsync(string userId)
    {
        var userGames = await _gameRepository.GetUserGamesAsync(userId);

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

    public async Task<bool> UserHasGameAsync(string userId, int rawgId)
    {
        return await _context.UserGames
            .Include(ug => ug.Game)
            .AnyAsync(ug => ug.UserId == userId && ug.Game.RawgId == rawgId);
    }

    public async Task<UserGameDetailsDto?> GetUserGameDetailsAsync(string userId, int rawgId)
    {
        // Get the UserGame along with the Game (to check RawgId)
        var userGame = await _context.UserGames
            .Include(ug => ug.Game)
            .FirstOrDefaultAsync(ug =>
                ug.UserId == userId &&
                ug.Game.RawgId == rawgId);

        if (userGame == null)
            return null;

        // Now separately query the Review
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == userGame.GameId);

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

    public async Task<UserGameWithReviewDto?> GetUserGameWithReviewAsync(string userId, int rawgId)
    {
        var userGame = await _context.UserGames
            .Include(ug => ug.Game)
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.Game.RawgId == rawgId);

        if (userGame == null) return null;

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == userGame.GameId);

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
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);

            if (string.IsNullOrWhiteSpace(reviewContent))
            {
                if (existingReview != null)
                {
                    _context.Reviews.Remove(existingReview);
                    await _context.SaveChangesAsync();
                }
                return true; // No review to add/update, but operation is successful
            }

            if (existingReview != null)
            {
                existingReview.UpdateReview(reviewContent, isRecommended);
            }
            else
            {
                var newReview = new Review(userId, gameId, reviewContent, isRecommended);
                await _context.Reviews.AddAsync(newReview);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            // Log exception if you have logging
            return false;
        }
    }

}
