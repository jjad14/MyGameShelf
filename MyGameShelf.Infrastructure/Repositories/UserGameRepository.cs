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

namespace MyGameShelf.Infrastructure.Repositories;
public class UserGameRepository : IUserGameRepository
{
    private readonly ApplicationDbContext _context;

    public UserGameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserGameWithFavoriteStatus>> GetUserGamesAsync(string userId, string? status, string? sort, int page = 1, int pageSize = 10)
    {
        var query = _context.UserGames
            .Include(ug => ug.Game)
            .Where(ug => ug.UserId == userId);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<GameStatus>(status, out var parsedStatus))
        {
            query = query.Where(ug => ug.Status == parsedStatus);
        }

        query = sort switch
        {
            "name" => query.OrderBy(ug => ug.Game.Name),
            "rating" => query.OrderByDescending(ug => ug.Rating),
            _ => query.OrderByDescending(ug => ug.AddedOn) // default to Recently Added
        };

        var userGames =  await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get favorite game IDs for this user and page games
        var gameIds = userGames.Select(ug => ug.GameId).ToList();

        // projection to include favorite status
        var favoritedGameIds = await _context.Favorites
            .Where(f => f.UserId == userId && gameIds.Contains(f.GameId))
            .Select(f => f.GameId)
            .ToListAsync();

        return userGames.Select(ug => new UserGameWithFavoriteStatus
        {
            UserGame = ug,
            IsFavorited = favoritedGameIds.Contains(ug.GameId)
        });
    }

    public async Task<int> CountByStatusAsync(string userId, string status)
    {
        if (String.IsNullOrEmpty(status))
        {
            return await _context.UserGames
                .Where(ug => ug.UserId == userId)
                .CountAsync();
        }

        if (!Enum.TryParse<GameStatus>(status, true, out var statusEnum))
        {
            return 0;
        }

        return await _context.UserGames
            .Where(ug => ug.UserId == userId && ug.Status == statusEnum)
            .CountAsync();
    }

    public async Task<UserGame?> GetUserGameAsync(string userId, int gameId)
    {
        var userGame = await _context.UserGames
            .Include(ug => ug.Game)
            .FirstOrDefaultAsync(ug =>
                ug.UserId == userId &&
                ug.Game.RawgId == gameId);

        return userGame;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public Task AddUserGame(UserGame userGame)
    {
        _context.UserGames.Add(userGame);

        return Task.CompletedTask;
    }

    public Task DeleteUserGame(UserGame userGame)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserGame(string userId, int gameId, string content, bool isRecommended)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckUserGameExists(string userId, int gameId)
    {
        return await _context.UserGames
            .Include(ug => ug.Game)
            .AnyAsync(ug => ug.UserId == userId && ug.Game.RawgId == gameId);
    }

}
