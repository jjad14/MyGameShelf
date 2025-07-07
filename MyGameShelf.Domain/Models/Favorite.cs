using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Favorite
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; }

    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game Game { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime FavoritedOn { get; set; } = DateTime.UtcNow;

    public Favorite(string userId, int gameId)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        GameId = gameId;
        CreatedAt = DateTime.UtcNow;
        FavoritedOn = DateTime.UtcNow;
    }

}
