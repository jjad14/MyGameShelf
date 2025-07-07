using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IFavoriteGameRepository
{
    Task<Favorite?> GetByUserAndGameAsync(string userId, int gameId);
    Task<bool> FavoriteExistsAsync(string userId, int gameId);
    Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId, int page = 1, int pageSize = 10);
    Task AddFavorite(Favorite favorite);
    Task DeleteFavorite(Favorite favorite);
    Task<bool> SaveChangesAsync();
}
