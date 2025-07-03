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
public class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _context;

    public GameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByRawgIdAsync(int rawgId)
    {
        return await _context.Games
            .Include(g => g.GameDevelopers).ThenInclude(gd => gd.Developer)
            .Include(g => g.GamePublishers).ThenInclude(gp => gp.Publisher)
            .Include(g => g.Platforms).ThenInclude(gp => gp.Platform)
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
            .Include(g => g.GameTags).ThenInclude(gt => gt.Tag)
            .FirstOrDefaultAsync(g => g.RawgId == rawgId);
    }

    public async Task<bool> AddGameAsync(Game game)
    {
        await _context.Games.AddAsync(game);

        return await SaveChangesAsync();
    }

    public async Task<bool> RemoveGameAsync(Game game)
    {
        _context.Games.Remove(game);

        return await SaveChangesAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<UserGame>> GetUserGamesAsync(string userId)
    {
        return await _context.UserGames
            .Include(ug => ug.Game)
                .ThenInclude(g => g.GameGenres)
                    .ThenInclude(gg => gg.Genre)
            .Include(ug => ug.Game)
                .ThenInclude(g => g.Platforms)
                    .ThenInclude(gp => gp.Platform)
            .Include(ug => ug.Game)
                .ThenInclude(g => g.GameTags)
                    .ThenInclude(gt => gt.Tag)
            .Include(ug => ug.Game)
                .ThenInclude(g => g.GameDevelopers)
                    .ThenInclude(gd => gd.Developer)
            .Include(ug => ug.Game)
                .ThenInclude(g => g.GamePublishers)
                    .ThenInclude(gp => gp.Publisher)
            .Where(ug => ug.UserId == userId)
            .ToListAsync();
    }
}
