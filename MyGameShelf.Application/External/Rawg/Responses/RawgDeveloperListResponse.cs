using MyGameShelf.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Application.External.Rawg.Responses;
public class RawgDeveloperListResponse
{
    public int Count { get; set; }
    public string Next { get; set; }
    public string Previous { get; set; }
    public List<RawgDeveloperSummary> Results { get; set; }
}

public class RawgDeveloperSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);

    [JsonPropertyName("games_count")]
    public int GamesCount { get; set; }

    [JsonPropertyName("image_background")]
    public string? ImageBackground { get; set; }
    public string ImageBackgroundOrDefault => ImageBackground ?? "/assets/img/game_wide_default.jpg";
}