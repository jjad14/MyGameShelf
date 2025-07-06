using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IUserGameRepository
{
    Task<IEnumerable<UserGame>> GetUserGamesAsync(string userId, string? status, string? sort, int page = 1, int pageSize = 10);
    Task<int> CountByStatusAsync(string userId, string status);
    Task<bool> CheckUserGameExists(string userId, int gameId);
    Task<UserGame?> GetUserGameAsync(string userId, int gameId);
    Task AddUserGame(UserGame userGame);
    Task UpdateUserGame(string userId, int gameId, string content, bool isRecommended);
    Task DeleteUserGame(UserGame userGame);
    Task<bool> SaveChangesAsync();
}
