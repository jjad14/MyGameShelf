using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Game
{
    [Key]
    public int Id { get; set; }
    public int RawgId { get; set; }

    // web url 
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime? Released { get; set; }
    public string? BackgroundImage { get; set; }

    public double? Rating { get; set; }
    public int? RatingsCount { get; set; }

    public int? SuggestionsCount { get; set; }
    public int? Metacritic { get; set; }
    public int? Playtime { get; set; }
    public DateTime? Updated { get; set; }
    public string? EsrbRating { get; set; }

    public ICollection<GamePlatform> Platforms { get; set; }
    public ICollection<GameDeveloper> GameDevelopers { get; set; }
    public ICollection<GamePublisher> GamePublishers { get; set; }
    public ICollection<GameGenre> GameGenres { get; set; }
    public ICollection<GameTag> GameTags { get; set; }

    // One game, many user games
    public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

}
