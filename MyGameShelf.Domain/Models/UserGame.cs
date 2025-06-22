using MyGameShelf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;

// Represents the relationship between a user and a game in the system
public class UserGame
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; }

    public GameStatus Status { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.UtcNow;

    public double? Difficulty { get; set; } // Global difficulty score (1-10) aggregated from user inputs
}
