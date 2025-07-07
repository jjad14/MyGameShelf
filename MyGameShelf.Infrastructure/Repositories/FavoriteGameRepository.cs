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

    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId, int page = 1, int pageSize = 10)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(r => r.Game)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
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

    public async Task<Favorite?> GetByUserAndGameAsync(string userId, int gameId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.GameId == gameId);
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
