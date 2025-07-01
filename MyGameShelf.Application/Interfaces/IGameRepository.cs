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
    Task AddGameAsync(Game game);
    Task RemoveGameAsync(Game game);
    Task SaveChangesAsync();
}