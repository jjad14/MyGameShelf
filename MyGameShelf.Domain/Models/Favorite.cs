using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Favorite
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } 

    public DateTime FavoritedOn { get; set; } = DateTime.UtcNow;
}
