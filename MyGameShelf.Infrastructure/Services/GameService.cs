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

    public async Task<bool> AddGameToUserAsync(string userId, int gameId, GameStatus status, int? difficulty, string? review, int? rating)
    {
        var exists = await _context.UserGames
            .AnyAsync(ug => ug.UserId == userId && ug.GameId == gameId);

        if (exists)
        {
            return false;
        }

        var userGame = new UserGame(userId, gameId)
        {
            Status = status,
        };

        userGame.SetDifficulty(difficulty);
        // TODO: Set rating and review if applicable

        _context.UserGames.Add(userGame);
        await _context.SaveChangesAsync();

        return true;
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

    public async Task<Game> UpdateGameToUserAsync(Game incomingGame)
    {
        var existingGame = await _gameRepository.GetByRawgIdAsync(incomingGame.RawgId);
        if (existingGame == null)
            throw new InvalidOperationException("Cannot update a game that doesn't exist.");

        // Example: add missing genres
        foreach (var gameGenre in incomingGame.GameGenres)
        {
            if (!existingGame.GameGenres.Any(g => g.Genre.Name.Equals(gameGenre.Genre.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == gameGenre.Genre.Name.ToLower());

                existingGame.GameGenres.Add(new GameGenre
                {
                    GameId = existingGame.Id,
                    Genre = existingGenre ?? gameGenre.Genre
                });
            }
        }

        // Repeat for Platforms, Tags, Developers, Publishers as needed

        await _gameRepository.SaveChangesAsync();
        return existingGame;
    }

}
