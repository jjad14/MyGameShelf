using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    // This connects the genre to the many games that belong to it
    public ICollection<GameGenre> GameGenres { get; set; }
}
