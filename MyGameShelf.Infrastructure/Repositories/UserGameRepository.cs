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
public class UserGameRepository : IUserGameRepository
{
    private readonly ApplicationDbContext _context;

    public UserGameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddUserGame(UserGame userGame)
    {
        _context.UserGames.Add(userGame);

        return Task.CompletedTask;
    }

    public async Task<bool> CheckUserGameExists(string userId, int gameId)
    {
        return await _context.UserGames
            .Include(ug => ug.Game)
            .AnyAsync(ug => ug.UserId == userId && ug.Game.RawgId == gameId);
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

    public Task DeleteUserGame(UserGame userGame)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserGame(string userId, int gameId, string content, bool isRecommended)
    {
        throw new NotImplementedException();
    }
}
