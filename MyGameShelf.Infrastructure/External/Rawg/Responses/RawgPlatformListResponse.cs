using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.External.Rawg.Responses;
public class RawgPlatformListResponse
{
    public int Count { get; set; }
    public string Next { get; set; }
    public string Previous { get; set; }
    public List<RawgPlatformSummary> Results { get; set; }
}

public class RawgPlatformSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }

    [JsonPropertyName("games_count")]
    public int GamesCount { get; set; }

    [JsonPropertyName("image_background")]
    public string ImageBackground { get; set; }

    [JsonPropertyName("year_start")]
    public int? YearStart { get; set; }

    [JsonPropertyName("year_end")]
    public int? YearEnd { get; set; }

}