using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class GameDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Metacritic { get; set; }

    public DateTime? Released { get; set; }
    public bool Tba { get; set; }
    public string BackgroundImage { get; set; }

    public string Website { get; set; }
    public double? Rating { get; set; }

    public string? EsrbRating { get; set; }
    public IEnumerable<string> Genres { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public IEnumerable<string> Platforms { get; set; }
    public IEnumerable<GameDetailsPublisherDto> Publishers { get; set; }
    public IEnumerable<string> Developers { get; set; }

    public IEnumerable<PlatformRequirementsDto>? PlatformRequirements { get; set; }
}

public class PlatformRequirementsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? MinimumRequirements { get; set; }
    public string? RecommendedRequirements { get; set; }
}