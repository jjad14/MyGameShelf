using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.External.Rawg.Responses;

// Reponses from RAWG API
public class RawgGenreListResponse
{
    public int Count { get; set; }
    public string Next { get; set; }
    public string Previous { get; set; }
    public List<RawgGenreSummary> Results { get; set; }
}

public class RawgGenreSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }

    [JsonPropertyName("games_count")]
    public int GamesCount { get; set; }

    [JsonPropertyName("image_background")]
    public string ImageBackground { get; set; }
}