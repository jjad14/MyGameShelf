using MyGameShelf.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Application.External.Rawg.Responses;

// Reponses from RAWG API

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
    public string Name { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);

    public DateTime? Released { get; set; }
    public bool Tba { get; set; }

    [JsonPropertyName("background_image")]
    public string BackgroundImage { get; set; }
    public string BackgroundImageOrDefault => BackgroundImage ?? "/assets/img/game_wide_default.jpg";

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
    public EsrbRating? EsrbRating { get; set; }
    public string EsrbRatingName => EsrbRating?.Name ?? "Not Rated";
    public string EsrbRatingSlug => EsrbRating?.Slug ?? "not-rated";

    public IEnumerable<RawgPlatformWrapper> Platforms { get; set; } = Enumerable.Empty<RawgPlatformWrapper>();
    public IEnumerable<RawgGenres> Genres { get; set; } = Enumerable.Empty<RawgGenres>();
    public IEnumerable<RawgTags> Tags { get; set; } = Enumerable.Empty<RawgTags>();
}

public class EsrbRating
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);
}

public class RawgPlatformWrapper
{
    public RawgPlatform Platform { get; set; }

    [JsonPropertyName("released_at")]
    public string? ReleasedAt { get; set; }

    [JsonPropertyName("requirements")]
    public RawgRequirements? Requirements { get; set; }
}

public class RawgPlatform
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);

    // Platform - Game data
    //--------------------------
    // Game platform image
    [JsonPropertyName("image_background")]
    public string? ImageBackground { get; set; }
    public string ImageBackgroundOrDefault => ImageBackground ?? "/assets/img/game_wide_default.jpg";
}

public class RawgRequirements
{
    public string? Minimum { get; set; }
    public string? Recommended { get; set; }
}

public class RawgGenres
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);
}

public class RawgTags
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);
    public string? Language { get; set; }
}