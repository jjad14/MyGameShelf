using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Review
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; }

    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game Game { get; set; }

    public string Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
