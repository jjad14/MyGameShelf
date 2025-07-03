using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IGameRepository
{
    Task<List<UserGame>> GetUserGamesAsync(string userId);
    Task<Game?> GetByRawgIdAsync(int rawgId);
    Task<bool> AddGameAsync(Game game);
    Task<bool> RemoveGameAsync(Game game);
    Task<bool> SaveChangesAsync();
}