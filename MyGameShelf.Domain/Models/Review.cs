using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Review
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; }

    public string Content { get; set; }
    public int Rating { get; set; } // Optional, if separate from RAWG rating
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
