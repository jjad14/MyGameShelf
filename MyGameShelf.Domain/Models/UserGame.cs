using MyGameShelf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;

// Represents the relationship between a user and a game in the system
public class UserGame
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game? Game { get; set; }

    public GameStatus Status { get; set; }

    public DateTime AddedOn { get; } = DateTime.UtcNow;

    public double? Difficulty { get; private set; } // Global difficulty score (1-10) aggregated from user inputs

    public double? Rating { get; private set; } // Global difficulty score (1-10) aggregated from user inputs

    // Enforce required initial state
    public UserGame(string userId, int gameId)
    {
        UserId = userId;
        GameId = gameId;
        Status = GameStatus.Playing; // enforced default
        AddedOn = DateTime.UtcNow;
    }

    // Parameterless constructor for EF Core
    private UserGame() { }

    public void SetRating(double? rating)
    {
        if (rating.HasValue)
        {
            if (rating < 1.0 || rating > 10.0)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1.0 and 10.0 inclusive.");
            }
        }

        Rating = rating;
    }

    public void SetDifficulty(double? difficulty)
    {
        if (difficulty.HasValue)
        {
            if (difficulty < 1.0 || difficulty > 10.0)
            {
                throw new ArgumentOutOfRangeException(nameof(difficulty), "Difficulty must be between 1.0 and 10.0 inclusive.");
            }
        }

        Difficulty = difficulty;
    }
}
