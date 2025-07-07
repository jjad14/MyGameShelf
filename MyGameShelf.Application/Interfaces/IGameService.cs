using MyGameShelf.Application.DTOs;
using MyGameShelf.Domain.Enums;
using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IGameService
{
    Task<IEnumerable<UserGameWithFavoriteStatus>> GetUserGamesAsync(string userId, string? status, string? sort, int page = 1, int pageSize = 10);
    Task<int> CountGamesByStatusAsync(string userId, string? status);
    Task<Dictionary<GameStatus, List<UserGameDto>>> GetUserGameListAsync(string userId);
    Task<Game?> AddGameMetadataAsync(Game incomingGame);
    Task<bool> AddGameToUserAsync(string userId, int gameId, GameStatus status, double? difficulty, string? review, double? rating, bool? recommended);
    Task<bool> UserHasGameAsync(string userId, int rawgId);
    Task<UserGameDetailsDto?> GetUserGameDetailsAsync(string userId, int rawgId);
    Task<UserGameWithReviewDto?> GetUserGameWithReviewAsync(string userId, int rawgId);

    Task<IEnumerable<Review>> GetUserReviewsAsync(string userId, string? sort, int page = 1, int pageSize = 10);
    Task<bool> UpdateOrAddReviewAsync(string userId, int gameId, string? reviewContent, bool isRecommended);
    Task<int> CountUserReviewsAsync(string userId);

    Task ToggleFavoriteGameAsync(string userId, int gameId);
    Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId, string? sort, int page = 1, int pageSize = 10);
    Task<int> CountUserFavoritesAsync(string userId);

}