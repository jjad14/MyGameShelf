using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Repositories;
public class FavoriteGameRepository : IFavoriteGameRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteGameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Favorite?> GetByUserAndGameAsync(string userId, int gameId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.GameId == gameId);
    }

    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId, string? sort, int page = 1, int pageSize = 10)
    {
        IQueryable<Favorite> query = _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Game);

        query = sort switch
        {
            "name" => query.OrderBy(f => f.Game.Name),
            "created" => query.OrderByDescending(f => f.CreatedAt),
            _ => query.OrderByDescending(f => f.CreatedAt)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountUserFavoritesAsync(string userId)
    {
        return await _context.Favorites
            .Where(r => r.UserId == userId)
            .CountAsync();
    }

    public Task AddFavorite(Favorite favorite)
    {
        _context.Favorites.Add(favorite);

        return Task.CompletedTask;
    }

    public Task DeleteFavorite(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);

        return Task.CompletedTask;
    }

    public async Task<bool> FavoriteExistsAsync(string userId, int gameId)
    {
        return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.GameId == gameId);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
