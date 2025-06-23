using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.External.Rawg.Responses;

public class RawgGameListResponse
{
    public int Count { get; set; }
    public string Next { get; set; }
    public string Previous { get; set; }
    public List<RawgGameSummary> Results { get; set; }
}

public class RawgGameSummary
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
    public DateTime? Released { get; set; }
    public bool Tba { get; set; }

    [JsonPropertyName("background_image")]
    public string BackgroundImage { get; set; }
    public double? Rating { get; set; }

    [JsonPropertyName("rating_top")]
    public int? RatingTop { get; set; }

    [JsonPropertyName("ratings_count")]
    public int? RatingsCount { get; set; }

    public int? Added { get; set; }

    [JsonPropertyName("suggestions_count")]
    public int? SuggestionsCount { get; set; }
    public int? Metacritic { get; set; }
    public int? Playtime { get; set; }
    public DateTime? Updated { get; set; }

    [JsonPropertyName("esrb_rating")]
    public EsrbRating EsrbRating { get; set; }
    public IEnumerable<RawgPlatformWrapper> Platforms { get; set; }
    public IEnumerable<RawgGenres> Genres { get; set; }
    public IEnumerable<RawgTags> Tags { get; set; }
}

public class EsrbRating
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
}

public class RawgPlatformWrapper
{
    public RawgPlatform Platform { get; set; }

    [JsonPropertyName("released_at")]
    public string ReleasedAt { get; set; }

    [JsonPropertyName("requirements_en")]
    public RawgRequirements Requirements { get; set; }
}

public class RawgPlatform
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public class RawgRequirements
{
    public string Minimum { get; set; }
    public string Recommended { get; set; }
}

public class RawgGenres
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public class RawgTags
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Language { get; set; }
}