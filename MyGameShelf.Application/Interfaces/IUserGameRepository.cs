using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IUserGameRepository
{
    Task<bool> CheckUserGameExists(string userId, int gameId);
    Task<UserGame?> GetUserGameAsync(string userId, int gameId);
    Task AddUserGame(UserGame userGame);
    Task UpdateUserGame(string userId, int gameId, string content, bool isRecommended);
    Task DeleteUserGame(UserGame userGame);
    Task<bool> SaveChangesAsync();
}
