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
    Task<Dictionary<GameStatus, List<UserGameDto>>> GetUserGameListAsync(string userId);
    Task<Game> AddGameMetadataAsync(Game incomingGame);
    Task<bool> AddGameToUserAsync(string userId, int gameId, GameStatus status, int? difficulty, string? review, int? rating, bool? recommended);
    Task<Game> UpdateGameToUserAsync(Game game);
}